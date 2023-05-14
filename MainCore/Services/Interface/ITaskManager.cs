using MainCore.Enums;
using MainCore.Tasks.Base;
using System.Collections.Generic;

namespace MainCore.Services.Interface
{
    public interface ITaskManager
    {
        void Add(int index, BotTask task, bool first = false);

        void Update(int index);

        void Remove(int index, BotTask task);

        void Clear(int index);

        BotTask GetCurrentTask(int index);

        int Count(int index);

        List<BotTask> GetList(int index);

        bool IsTaskExecuting(int index);

        AccountStatus GetAccountStatus(int index);

        void UpdateAccountStatus(int index, AccountStatus status);

        void StopCurrentTask(int index);
    }
}