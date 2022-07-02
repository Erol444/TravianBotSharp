using MainCore.Enums;
using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;

namespace MainCore.Services
{
    public interface ITaskManager
    {
        public void Add(int index, BotTask task);

        public BotTask Find(int index, Type type);

        public void Remove(int index, BotTask task);

        public void Clear(int index);

        public BotTask GetCurrentTask(int index);

        public int Count(int index);

        public List<BotTask> GetTaskList(int index);

        public bool IsTaskExecuting(int index);

        public AccountStatus GetAccountStatus(int index);

        public void UpdateAccountStatus(int index, AccountStatus status);
    }
}