using MainCore.Enums;
using MainCore.Models.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainCore.Services
{
    public class TaskManager : ITaskManager
    {
        public TaskManager(IDatabaseEvent databaseEvent, ILogManager logManager)
        {
            _databaseEvent = databaseEvent;
            _logManager = logManager;
            _databaseEvent.TaskExecuted += Loop;
        }

        public void Add(int index, BotTask task)
        {
            Check(index);
            _tasksDict[index].Add(task);
            _databaseEvent.OnTaskUpdated(index);
        }

        public void Clear(int index)
        {
            Check(index);
            _tasksDict[index].Clear();
            _databaseEvent.OnTaskUpdated(index);
        }

        public int Count(int index)
        {
            Check(index);
            return _tasksDict[index].Count;
        }

        public BotTask Find(int index, Type type)
        {
            Check(index);
            return _tasksDict[index].FirstOrDefault(x => x.GetType() == type);
        }

        public BotTask GetCurrentTask(int index)
        {
            return _tasksDict[index].FirstOrDefault(x => x.Stage == TaskStage.Executing);
        }

        public void Remove(int index, BotTask task)
        {
            Check(index);
            _tasksDict[index].Remove(task);
            _databaseEvent.OnTaskUpdated(index);
        }

        public List<BotTask> GetTaskList(int index)
        {
            Check(index);
            return _tasksDict[index];
        }

        private void Check(int index)
        {
            _tasksDict.TryAdd(index, new());
            _taskExecuting.TryAdd(index, false);
            _botStatus.TryAdd(index, AccountStatus.Offline);
        }

        private async void Loop()
        {
            var tasks = new List<Task>();
            foreach (var item in _tasksDict.Keys)
            {
                Check(item);
                if (_tasksDict[item].Count == 0) continue;

                tasks.Add(TaskExecute(item));
            }
            await Task.WhenAll(tasks);
        }

        private async Task TaskExecute(int index)
        {
            _botStatus.TryGetValue(index, out var accountStatus);
            if (accountStatus != AccountStatus.Online) return;

            _taskExecuting.TryGetValue(index, out var isTaskExcuting);
            if (isTaskExcuting) return;

            _taskExecuting.TryUpdate(index, true, false);

            var task = _tasksDict[index].First();

            _logManager.Information(index, $"{task.GetType().Name} is started");
            await task.Execute();
            _logManager.Information(index, $"{task.GetType().Name} is completed");

            Remove(index, task);

            _taskExecuting.TryUpdate(index, false, true);
        }

        public bool IsTaskExecuting(int index)
        {
            Check(index);
            _taskExecuting.TryGetValue(index, out var isTaskExcuting);
            return isTaskExcuting;
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
        private readonly ConcurrentDictionary<int, bool> _taskExecuting = new();
        private readonly Dictionary<int, AccountStatus> _botStatus = new();

        private readonly IDatabaseEvent _databaseEvent;
        private readonly ILogManager _logManager;
    }
}