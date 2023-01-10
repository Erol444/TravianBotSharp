using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Services.Interface;
using MainCore.Tasks;
using MainCore.Tasks.Misc;
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
        public TaskManager(IDbContextFactory<AppDbContext> contextFactory, IEventManager eventManager, ILogManager logManager)
        {
            _contextFactory = contextFactory;
            _eventManager = eventManager;
            _logManager = logManager;
            _eventManager.TaskExecute += Loop;
        }

        public void Add(int index, BotTask task, bool first = false)
        {
            Check(index);
            if (first)
            {
                var firstTask = _tasksDict[index].FirstOrDefault();
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
            _tasksDict[index].Add(task);
            ReOrder(index);
        }

        public void Update(int index)
        {
            Check(index);
            ReOrder(index);
        }

        public void Remove(int index, BotTask task)
        {
            Check(index);
            _tasksDict[index].Remove(task);
            ReOrder(index);
        }

        public void Clear(int index)
        {
            Check(index);
            _tasksDict[index].Clear();
            ReOrder(index);
        }

        public void ReOrder(int index)
        {
            Check(index);
            _tasksDict[index].Sort((x, y) => DateTime.Compare(x.ExecuteAt, y.ExecuteAt));
            _eventManager.OnTaskUpdate(index);
        }

        public int Count(int index)
        {
            Check(index);
            return _tasksDict[index].Count;
        }

        public BotTask GetCurrentTask(int index)
        {
            Check(index);
            return _tasksDict[index].FirstOrDefault(x => x.Stage == TaskStage.Executing);
        }

        public List<BotTask> GetList(int index)
        {
            Check(index);
            return _tasksDict[index].ToList();
        }

        private void Check(int index)
        {
            _tasksDict.TryAdd(index, new());
            _taskExecuting.TryAdd(index, false);
            _botStatus.TryAdd(index, AccountStatus.Offline);
            _cancellationTokenSources.TryAdd(index, null);
        }

        private void Loop(int index)
        {
            var accountStatus = _botStatus[index];
            if (accountStatus != AccountStatus.Online) return;
            if (_tasksDict[index].Count == 0) return;
            var task = _tasksDict[index].First();

            if (task.ExecuteAt > DateTime.Now) return;

            var retryPolicy = Policy
                .Handle<Exception>()
                .OrResult<Result>(x => x.HasError<Retry>())
                .WaitAndRetry(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(5), onRetry: (error, _, retryCount, _) =>
                {
                    _logManager.Warning(index, $"There is something wrong.");
                    if (error.Exception is null)
                    {
                        var errors = error.Result.Reasons.Select(x => x.Message).ToList();
                        _logManager.Error(index, string.Join(Environment.NewLine, errors));
                    }
                    else
                    {
                        var exception = error.Exception;
                        _logManager.Error(index, exception.Message, exception);
                    }
                    _logManager.Warning(index, $"Retry {retryCount} for {task.GetName()}");

                    if (task is AccountBotTask accountTask)
                    {
                        accountTask.RefreshChrome();
                    }
                });

            _taskExecuting[index] = true;
            task.Stage = TaskStage.Executing;
            _eventManager.OnTaskUpdate(index);
            var cacheExecuteTime = task.ExecuteAt;

            var cts = new CancellationTokenSource();
            _cancellationTokenSources[index] = cts;
            task.CancellationToken = cts.Token;

            _cancellationTokenSources[index] = cts;

            _logManager.Information(index, $"{task.GetName()} is started");
            ///===========================================================///
            var poliResult = retryPolicy.ExecuteAndCapture(task.Execute);
            ///===========================================================///
            _logManager.Information(index, $"{task.GetName()} is finished");

            if (poliResult.FinalException is not null)
            {
                UpdateAccountStatus(index, AccountStatus.Paused);
                _logManager.Warning(index, $"There is something wrong. Bot is pausing. Last exception is", task);
                var ex = poliResult.FinalException;
                _logManager.Error(index, ex.Message, ex);
            }
            else
            {
                var result = poliResult.Result;
                if (result.IsFailed)
                {
                    task.Stage = TaskStage.Waiting;

                    if (result.HasError<Login>())
                    {
                        _logManager.Warning(index, "Login page is showing, stop current task and login", task);
                        Add(index, new LoginTask(index), true);
                    }
                    else if (result.HasError<Stop>())
                    {
                        UpdateAccountStatus(index, AccountStatus.Paused);
                        _logManager.Warning(index, $"There is something wrong. Bot is pausing", task);
                        var errors = result.Reasons.Select(x => x.Message).ToList();
                        _logManager.Error(index, string.Join(Environment.NewLine, errors));
                    }
                    else if (result.HasError<Skip>())
                    {
                        if (task.ExecuteAt == cacheExecuteTime) Remove(index, task);
                        else
                        {
                            ReOrder(index);
                            _eventManager.OnTaskUpdate(index);
                        }
                    }
                    else if (result.HasError<Cancel>())
                    {
                        UpdateAccountStatus(index, AccountStatus.Paused);
                        _logManager.Information(index, $"Stop command requested", task);
                    }
                }
                else
                {
                    if (task.ExecuteAt == cacheExecuteTime) Remove(index, task);
                    else
                    {
                        ReOrder(index);
                        _eventManager.OnTaskUpdate(index);
                    }
                }
            }
            task.Stage = TaskStage.Waiting;
            _taskExecuting[index] = false;

            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(index);
            Thread.Sleep(Random.Shared.Next(setting.TaskDelayMin, setting.TaskDelayMax));

            cts.Dispose();
            _cancellationTokenSources[index] = null;
        }

        public bool IsTaskExecuting(int index)
        {
            Check(index);
            return _taskExecuting[index];
        }

        public AccountStatus GetAccountStatus(int index)
        {
            Check(index);
            _botStatus.TryGetValue(index, out var accountStatus);
            return accountStatus;
        }

        public void UpdateAccountStatus(int index, AccountStatus status)
        {
            Check(index);

            _botStatus[index] = status;
            _eventManager.OnStatusUpdate(index, status);
        }

        public void StopCurrentTask(int index)
        {
            Check(index);
            var cts = _cancellationTokenSources[index];
            if (cts is null) return;
            cts.Cancel();
        }

        private readonly Dictionary<int, List<BotTask>> _tasksDict = new();
        private readonly Dictionary<int, bool> _taskExecuting = new();
        private readonly Dictionary<int, AccountStatus> _botStatus = new();
        private readonly Dictionary<int, CancellationTokenSource> _cancellationTokenSources = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ILogManager _logManager;
    }
}