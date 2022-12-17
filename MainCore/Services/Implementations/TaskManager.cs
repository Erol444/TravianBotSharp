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
        }

        private void Loop(int index)
        {
            var accountStatus = _botStatus[index];
            if (accountStatus != AccountStatus.Online) return;
            if (_tasksDict[index].Count == 0) return;
            var task = _tasksDict[index].First();

            if (task.ExecuteAt > DateTime.Now) return;
            _taskExecuting[index] = true;
            task.Stage = TaskStage.Executing;
            _eventManager.OnTaskUpdate(index);
            _logManager.Information(index, $"{task.Name} is started");
            var cacheExecuteTime = task.ExecuteAt;

            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(index);

            var retryPolicy = Policy.HandleResult<Result>(x => x.HasError<MustRetry>())
                .WaitAndRetry(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(5), onRetry: (error, retryCount, context) =>
            {
                _logManager.Warning(index, $"There is something wrong.");
                var errors = error.Result.Reasons.Select(x => x.Message).ToList();
                _logManager.Error(index, string.Join(Environment.NewLine, errors));
                _logManager.Warning(index, $"Retry {retryCount} for {task.Name}");
            });

            var poliResult = retryPolicy.ExecuteAndCapture(task.Execute);
            if (poliResult.FinalException is not null)
            {
                var ex = poliResult.FinalException;
                _logManager.Error(index, ex.Message, ex);
            }
            else
            {
                var result = poliResult.Result;
                if (result.IsFailed)
                {
                    if (result.HasError<NeedLogin>())
                    {
                        _logManager.Warning(index, "Login page is showing, stop current task and login");
                        Add(index, new LoginTask(index), true);
                    }
                    else if (result.HasError<MustStop>())
                    {
                        UpdateAccountStatus(index, AccountStatus.Paused);
                        _logManager.Warning(index, $"There is something wrong. Bot is pausing");
                        var errors = result.Reasons.Select(x => x.Message).ToList();
                        _logManager.Error(index, string.Join(Environment.NewLine, errors));
                    }
                }
                else
                {
                }
            }

            _logManager.Information(index, $"{task.Name} is finished");
            if (task.ExecuteAt == cacheExecuteTime) Remove(index, task);
            else
            {
                task.Stage = TaskStage.Waiting;
                ReOrder(index);
            }

            _eventManager.OnTaskUpdate(index);
            _taskExecuting[index] = false;

            Thread.Sleep(Random.Shared.Next(setting.TaskDelayMin, setting.TaskDelayMax));
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

        private readonly Dictionary<int, List<BotTask>> _tasksDict = new();
        private readonly Dictionary<int, bool> _taskExecuting = new();
        private readonly Dictionary<int, AccountStatus> _botStatus = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly ILogManager _logManager;
    }
}