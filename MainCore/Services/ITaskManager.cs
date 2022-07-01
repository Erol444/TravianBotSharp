using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;

namespace MainCore.Services
{
    public interface ITaskManager
    {
        public event Action TaskUpdate;

        public void Add(int index, BotTask task);

        public BotTask Find(int index, Type type);

        public void Remove(int index, BotTask task);

        public void Clear(int index);

        public BotTask GetCurrentTask(int index);

        public int Count(int index);

        public List<BotTask> GetTaskList(int index);
    }
}