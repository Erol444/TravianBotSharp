using MainCore.Models.Runtime;
using System.Collections.ObjectModel;

namespace MainCore.Services
{
    public class TaskManager : ITaskManager
    {
        public event Action TaskUpdate;

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

        public ObservableCollection<BotTask> GetTaskList(int index)
        {
            Check(index);
            return _tasksDict[index];
        }

        private void Check(int index)
        {
            if (!_tasksDict.TryGetValue(index, out _))
            {
                _tasksDict.Add(index, new());
            }
        }

        private readonly Dictionary<int, ObservableCollection<BotTask>> _tasksDict = new();
    }
}