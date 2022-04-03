using System;
using System.Linq;
using System.Collections.Generic;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks;
using static TbsCore.Tasks.BotTask;
using Newtonsoft.Json;
using System.IO;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Models.AccModels
{
    public class TaskList
    {
        public delegate void TaskUpdated();

        private readonly List<BotTask> _tasks;
        private readonly Account _account;
        public TaskUpdated OnUpdateTask;

        public TaskList(Account account)
        {
            _tasks = new List<BotTask>();
            _account = account;
        }

        public void Add(BotTask task, bool IfNotExists = false, Village vill = null)
        {
            if (IfNotExists && IsTaskExists(task.GetType(), vill))
                return;

            if (task.ExecuteAt == null) task.ExecuteAt = DateTime.Now;

            _tasks.Add(task);
            ReOrder();
        }

        public void Add(List<BotTask> taskList)
        {
            taskList.ForEach((task) => Add(task));
            ReOrder();
        }

        public BotTask FindTask(Type typeTask, Village vill = null)
        {
            if (vill == null)
            {
                return _tasks.FirstOrDefault(x => x.GetType() == typeTask);
            }

            return _tasks.FirstOrDefault(x => x.Vill == vill && x.GetType() == typeTask);
        }

        public List<BotTask> FindTasks(Type typeTask, Village vill = null)
        {
            if (vill == null)
            {
                return _tasks.Where(x => x.GetType() == typeTask).ToList();
            }

            return _tasks.Where(x => x.Vill == vill && x.GetType() == typeTask).ToList();
        }

        public BotTask FindTaskBasedPriority(TaskPriority priority)
        {
            switch (priority)
            {
                case TaskPriority.High:
                    return _tasks.FirstOrDefault(x => x.Priority == TaskPriority.High);

                case TaskPriority.Medium:
                    return _tasks.FirstOrDefault(x =>
                        x.Priority == TaskPriority.High ||
                        x.Priority == TaskPriority.Medium
                    );

                case TaskPriority.Low:
                    return _tasks.FirstOrDefault();
            }
            return null;
        }

        public void Remove(BotTask task)
        {
            _tasks.Remove(task);
            ReOrder();
        }

        public void Remove(Type typeTask, Village vill = null, int timeBelow = 0, BotTask thisTask = null)
        {
            var removeTasks = _tasks.Where(x => x.GetType() == typeTask);

            // if vill is specificed, pick task from that vill
            if (vill != null)
            {
                removeTasks = removeTasks.Where(x => x.Vill == vill);
            }

            // if thisTask is specificed, dont remove it
            if (thisTask != null)
            {
                removeTasks = removeTasks.Where(x => x != thisTask);
            }

            // if timeBelow is specificed, pick task has time excuted in next "timeBelow" mins
            if (timeBelow > 0)
            {
                removeTasks = removeTasks.Where(x => x.ExecuteAt < DateTime.Now.AddMinutes(timeBelow));
            }

            _tasks.RemoveAll(x => removeTasks.Contains(x));
            ReOrder();
        }

        public void Clear()
        {
            _tasks.Clear();
            OnUpdateTask?.Invoke();
        }

        public void ReOrder()
        {
            _tasks.Sort((a, b) => DateTime.Compare(a.ExecuteAt, b.ExecuteAt));
            OnUpdateTask?.Invoke();
        }

        public bool IsTaskExists(Type typeTask, Village vill = null)
        {
            if (vill != null)
            {
                return _tasks.Any(x => x.GetType() == typeTask && x.Vill == vill);
            }
            return _tasks.Any(x => x.GetType() == typeTask);
        }

        public int Count
        {
            get
            {
                return _tasks.Count;
            }
        }

        public List<BotTask> ToList()
        {
            return _tasks.ToList();
        }

        public BotTask FirstTask
        {
            get => _tasks.FirstOrDefault();
        }

        public void Save()
        {
            var list = new List<TaskFileModel>();
            foreach (var task in _tasks)
            {
                if (task.Stage == TaskStage.Executing) task.Stage = TaskStage.Start;
                list.Add(new TaskFileModel()
                {
                    Type = task.GetType().ToString(),
                    Content = JsonConvert.SerializeObject(task),
                });
            }

            File.WriteAllText(IoHelperCore.UserTaskPath(_account.AccInfo.Nickname, _account.AccInfo.ServerUrl), JsonConvert.SerializeObject(list));
        }

        public void Load()
        {
            if (!IoHelperCore.UserTaskExists(_account.AccInfo.Nickname, _account.AccInfo.ServerUrl))
                return;
            var str = File.ReadAllText(IoHelperCore.UserTaskPath(_account.AccInfo.Nickname, _account.AccInfo.ServerUrl));
            var list = JsonConvert.DeserializeObject<List<TaskFileModel>>(str);
            foreach (var task in list)
            {
                var type = Type.GetType(task.Type);
                if (type == typeof(TimeSleep))
                {
                    continue;
                }
                var botTask = JsonConvert.DeserializeObject(task.Content, type) as BotTask;
                if (botTask.Stage == TaskStage.Executing) botTask.Stage = TaskStage.Start;
                if (botTask.Vill != null)
                {
                    var vill = _account.Villages.FirstOrDefault(x => x.Id == botTask?.Vill.Id);
                    if (vill != null) continue;
                    botTask.Vill = vill;
                }
                _tasks.Add(botTask);
            }
        }
    }

    public class TaskFileModel
    {
        public string Type { get; set; }
        public string Content { get; set; }
    }
}