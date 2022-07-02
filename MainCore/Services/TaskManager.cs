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
        public event Action TaskUpdate;

        public TaskManager(IDatabaseEvent databaseEvent, ITimerManager timerManager)
        {
            _databaseEvent = databaseEvent;
            _timerManager = timerManager;
            _timerManager.TaskExecute += Loop;
        }

        public void Add(int index, BotTask task)
        {
            Check(index);
            _tasksDict[index].Add(task);
            TaskUpdate?.Invoke();
        }

        public void Clear(int index)
        {
            Check(index);
            _tasksDict[index].Clear();
            TaskUpdate?.Invoke();
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
            throw new NotImplementedException();
        }

        public void Remove(int index, BotTask task)
        {
            Check(index);
            _tasksDict[index].Remove(task);
            TaskUpdate?.Invoke();
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

            //await _tasksDict[index].First().Execute();
            await Task.Delay(1000);
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

        private readonly ITimerManager _timerManager;
    }
}