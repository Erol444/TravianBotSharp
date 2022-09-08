using MainCore.Enums;
using MainCore.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Services
{
    public class TaskManager : ITaskManager
    {
        public TaskManager(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IEventManager databaseEvent, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            _contextFactory = contextFactory;
            _eventManager = databaseEvent;
            _chromeManager = chromeManager;
            _logManager = logManager;
            _planManager = planManager;
            _restClientManager = restClientManager;
            _eventManager.TaskExecuted += Loop;
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
                    ReOrder(index);
                }
            }
            if (task.ExecuteAt == default) task.ExecuteAt = DateTime.Now;
            task.SetService(_contextFactory, _chromeManager.Get(index), this, _eventManager, _logManager, _planManager, _restClientManager);

            _tasksDict[index].Add(task);
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
            _eventManager.OnTaskUpdated(index);
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
            task.Cts = new();
            _eventManager.OnTaskUpdated(index);
            _logManager.Information(index, $"{task.Name} is started");
            var cacheExecuteTime = task.ExecuteAt;
            try
            {
                task.Execute();
            }
            catch (Exception e)
            {
                _logManager.Error(index, e.Message, e);
                if (task.RetryCounter > 3)
                {
                    _logManager.Information(index, $"{task.Name} was excuted 3 times. Bot is paused");
                    UpdateAccountStatus(index, AccountStatus.Paused);
                }
                else
                {
                    _logManager.Information(index, $"{task.Name} is failed. Retry counter is increased ({task.RetryCounter})");
                    task.RetryCounter++;
                    task.Cts.Cancel();
                    var chrome = task.ChromeBrowser;
                    chrome.GetChrome().Navigate().Refresh();
                    var wait = chrome.GetWait();
                    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                }
            }
            var isCancellationRequested = task.Cts.IsCancellationRequested;
            task.Cts.Dispose();
            if (isCancellationRequested)
            {
                _logManager.Information(index, $"{task.Name} is stopped and reset.");
                task.Stage = TaskStage.Waiting;
            }
            else
            {
                _logManager.Information(index, $"{task.Name} is finished");
                if (task.ExecuteAt == cacheExecuteTime) Remove(index, task);
                else
                {
                    task.Stage = TaskStage.Waiting;
                    ReOrder(index);
                }
            }

            _eventManager.OnTaskUpdated(index);
            _taskExecuting[index] = false;

            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(index);

            Thread.Sleep(_rand.Next(setting.TaskDelayMin, setting.TaskDelayMax));
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
            _eventManager.OnAccountStatusUpdate();
        }

        private readonly Dictionary<int, List<BotTask>> _tasksDict = new();
        private readonly Dictionary<int, bool> _taskExecuting = new();
        private readonly Dictionary<int, AccountStatus> _botStatus = new();
        private readonly Random _rand = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;
        private readonly IChromeManager _chromeManager;
        private readonly ILogManager _logManager;
        private readonly IPlanManager _planManager;
        private readonly IRestClientManager _restClientManager;
    }
}