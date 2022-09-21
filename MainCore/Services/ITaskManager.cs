using MainCore.Enums;
using MainCore.Tasks;
using System.Collections.Generic;

namespace MainCore.Services
{
    public interface ITaskManager
    {
        public void Add(int index, BotTask task, bool first = false);

        public void Update(int index);

        public void Remove(int index, BotTask task);

        public void Clear(int index);

        public BotTask GetCurrentTask(int index);

        public int Count(int index);

        public List<BotTask> GetList(int index);

        public bool IsTaskExecuting(int index);

        public AccountStatus GetAccountStatus(int index);

        public void UpdateAccountStatus(int index, AccountStatus status);
    }
}