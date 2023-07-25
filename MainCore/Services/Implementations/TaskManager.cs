using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using MainCore.Tasks.FunctionTasks;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Services.Implementations
{
    public sealed class TaskManager : ITaskManager
    {
        private class TaskInfo
        {
            public bool IsExecuting { get; set; } = false;
            public AccountStatus Status { get; set; } = AccountStatus.Offline;
            public CancellationTokenSource CancellationTokenSource { get; set; } = null;
        }

        private readonly Dictionary<int, List<BotTask>> _tasksDict = new();
        private readonly Dictionary<int, TaskInfo> _taskInfo = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ILogHelper _logHelper;
        private readonly IChromeManager _chromeManager;

        public TaskManager(IDbContextFactory<AppDbContext> contextFactory, IEventManager eventManager, ILogHelper logHelper, IChromeManager chromeManager)
        {
            _contextFactory = contextFactory;
            _eventManager = eventManager;
            _logHelper = logHelper;
            _eventManager.TaskExecute += Loop;
            _chromeManager = chromeManager;
        }

        public void Add<T>(int accountId, bool first = false) where T : AccountBotTask
        {
            var task = (T)Activator.CreateInstance(typeof(T), accountId, null);
            Add(accountId, task, first);
        }

        public void Add<T>(int accountId, int villageId, bool first = false) where T : VillageBotTask
        {
            var task = (T)Activator.CreateInstance(typeof(T), villageId, accountId, null);
            Add(accountId, task, first);
        }

        public void Add<T>(int accountId, Func<T> func, bool first = false) where T : BotTask
        {
            var task = func();
            Add(accountId, task, first);
        }

        private void Add(int accountId, BotTask task, bool first = false)
        {
            var tasks = GetTasks(accountId);

            if (first)
            {
                var firstTask = tasks.FirstOrDefault();
                if (firstTask is not null)
                {
                    if (firstTask.ExecuteAt < DateTime.Now)
                    {
                        task.ExecuteAt = firstTask.ExecuteAt.AddSeconds(-1);
                    }
                    else
                    {
                        task.ExecuteAt = DateTime.Now;
                    }
                }
            }
            if (task.ExecuteAt == default) task.ExecuteAt = DateTime.Now;

            tasks.Add(task);
            ReOrder(accountId, tasks);
        }

        public void Remove(int accountId, BotTask task)
        {
            var tasks = GetTasks(accountId);

            tasks.Remove(task);
            ReOrder(accountId, tasks);
        }

        public void Clear(int accountId)
        {
            var tasks = GetTasks(accountId);
            tasks.Clear();
            _eventManager.OnTaskUpdate(accountId);
        }

        public void ReOrder(int accountId)
        {
            var tasks = GetTasks(accountId);
            ReOrder(accountId, tasks);
        }

        private void ReOrder(int accountId, List<BotTask> tasks)
        {
            tasks.Sort((x, y) => DateTime.Compare(x.ExecuteAt, y.ExecuteAt));
            _eventManager.OnTaskUpdate(accountId);
        }

        public int Count(int accountId)
        {
            var tasks = GetTasks(accountId);
            return tasks.Count;
        }

        public BotTask GetCurrentTask(int accountId)
        {
            var tasks = GetTasks(accountId);
            return tasks.FirstOrDefault(x => x.Stage == TaskStage.Executing);
        }

        public List<BotTask> GetList(int accountId)
        {
            var tasks = GetTasks(accountId);
            return tasks.ToList();
        }

        private void Loop(int accountId)
        {
            var info = GetInfo(accountId);
            if (info.Status != AccountStatus.Online) return;
            var tasks = GetTasks(accountId);
            if (tasks.Count == 0) return;
            var task = tasks.First();

            if (task.ExecuteAt > DateTime.Now) return;

            var retryPolicy = Policy
                .Handle<Exception>()
                .OrResult<Result>(x => x.HasError<Retry>())
                .WaitAndRetry(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(5), onRetry: (error, _, retryCount, _) =>
                {
                    _logHelper.Warning(accountId, $"There is something wrong.");
                    if (error.Exception is null)
                    {
                        var errors = error.Result.Reasons.Select(x => x.Message).ToList();
                        _logHelper.Error(accountId, string.Join(Environment.NewLine, errors));
                    }
                    else
                    {
                        var exception = error.Exception;
                        _logHelper.Error(accountId, exception.Message, exception);
                    }
                    _logHelper.Warning(accountId, $"Retry {retryCount} for {task.GetName()}");
                    var chromeBrowser = _chromeManager.Get(accountId);
                    chromeBrowser.Navigate();
                });

            info.IsExecuting = true;
            task.Stage = TaskStage.Executing;
            _eventManager.OnTaskUpdate(accountId);

            var cacheExecuteTime = task.ExecuteAt;

            var cts = new CancellationTokenSource();
            info.CancellationTokenSource = cts;
            task.CancellationToken = cts.Token;

            _logHelper.Information(accountId, $"{task.GetName()} is started");
            ///===========================================================///
            var poliResult = retryPolicy.ExecuteAndCapture(task.Execute);
            ///===========================================================///
            _logHelper.Information(accountId, $"{task.GetName()} is finished");

            if (poliResult.FinalException is not null)
            {
                UpdateAccountStatus(accountId, AccountStatus.Paused);
                _logHelper.Warning(accountId, $"There is something wrong. Bot is pausing. Last exception is", task);
                var ex = poliResult.FinalException;
                _logHelper.Error(accountId, ex.Message, ex);
            }
            else
            {
                var result = poliResult.Result ?? poliResult.FinalHandledResult;
                if (result.IsFailed)
                {
                    if (result.HasError<Login>())
                    {
                        Add<LoginTask>(accountId, true);
                        _logHelper.Warning(accountId, result.Reasons[0].Message, task);
                    }
                    else if (result.HasError<Stop>())
                    {
                        UpdateAccountStatus(accountId, AccountStatus.Paused);
                        _logHelper.Warning(accountId, result.Reasons[0].Message, task);
                    }
                    else if (result.HasError<Skip>())
                    {
                        if (task.ExecuteAt == cacheExecuteTime)
                        {
                            Remove(accountId, task);
                        }
                        _logHelper.Warning(accountId, result.Reasons[0].Message, task);
                    }
                    else if (result.HasError<Cancel>())
                    {
                        UpdateAccountStatus(accountId, AccountStatus.Paused);
                        _logHelper.Warning(accountId, result.Reasons[0].Message, task);
                    }
                    else
                    {
                        var errors = result.Reasons.Select(x => x.Message).ToList();
                        _logHelper.Warning(accountId, string.Join(Environment.NewLine, errors), task);
                    }
                }
                else
                {
                    if (task.ExecuteAt == cacheExecuteTime)
                    {
                        Remove(accountId, task);
                    }
                }
            }

            task.Stage = TaskStage.Waiting;
            info.IsExecuting = false;
            ReOrder(accountId);

            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            Thread.Sleep(Random.Shared.Next(setting.TaskDelayMin, setting.TaskDelayMax));

            cts.Dispose();
            info.CancellationTokenSource = null;
        }

        public bool IsTaskExecuting(int accountId)
        {
            var info = GetInfo(accountId);
            return info.IsExecuting;
        }

        public AccountStatus GetAccountStatus(int accountId)
        {
            var info = GetInfo(accountId);
            return info.Status;
        }

        public void UpdateAccountStatus(int accountId, AccountStatus status)
        {
            var info = GetInfo(accountId);
            info.Status = status;
            _eventManager.OnStatusUpdate(accountId, status);
        }

        public void StopCurrentTask(int accountId)
        {
            var info = GetInfo(accountId);
            var cts = info.CancellationTokenSource;
            if (cts is null) return;
            cts.Cancel();
        }

        private List<BotTask> GetTasks(int accountId)
        {
            var tasks = _tasksDict.GetValueOrDefault(accountId);
            if (tasks is null)
            {
                tasks = new();
                _tasksDict.Add(accountId, tasks);
            }
            return tasks;
        }

        private TaskInfo GetInfo(int accountId)
        {
            var info = _taskInfo.GetValueOrDefault(accountId);
            if (info is null)
            {
                info = new();
                _taskInfo.Add(accountId, info);
            }

            return info;
        }
    }
}