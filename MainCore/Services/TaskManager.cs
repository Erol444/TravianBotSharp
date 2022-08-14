using MainCore.Enums;
using MainCore.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Services
{
    public class TaskManager : ITaskManager
    {
        public TaskManager(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IEventManager databaseEvent, ILogManager logManager, IPlanManager planManager)
        {
            _contextFactory = contextFactory;
            _databaseEvent = databaseEvent;
            _chromeManager = chromeManager;
            _logManager = logManager;
            _planManager = planManager;
            _databaseEvent.TaskExecuted += Loop;
        }

        public void Add(int index, BotTask task)
        {
            Check(index);
            if (task.ExecuteAt == default) task.ExecuteAt = DateTime.Now;

            task.ContextFactory = _contextFactory;
            task.DatabaseEvent = _databaseEvent;
            task.TaskManager = this;
            task.LogManager = _logManager;
            task.ChromeBrowser = _chromeManager.Get(task.AccountId);
            task.PlanManager = _planManager;

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
            _databaseEvent.OnTaskUpdated(index);
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

            _taskExecuting[index] = true;
            task.Stage = TaskStage.Executing;
            _databaseEvent.OnTaskUpdated(index);
            _logManager.Information(index, $"{task.Name} is started");
            try
            {
                task.Execute();
            }
            catch (Exception e)
            {
                _ = e;
                //UpdateAccountStatus(index, AccountStatus.Paused);
            }
            _logManager.Information(index, $"{task.GetType().Name} is completed");

            if (task.ExecuteAt < DateTime.Now) Remove(index, task);
            else
            {
                task.Stage = TaskStage.Start;
                ReOrder(index);
            }
            _databaseEvent.OnTaskUpdated(index);
            _taskExecuting[index] = false;
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
            _databaseEvent.OnAccountStatusUpdate();
        }

        private readonly Dictionary<int, List<BotTask>> _tasksDict = new();
        private readonly Dictionary<int, bool> _taskExecuting = new();
        private readonly Dictionary<int, AccountStatus> _botStatus = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _databaseEvent;
        private readonly IChromeManager _chromeManager;
        private readonly ILogManager _logManager;
        private readonly IPlanManager _planManager;
    }
}