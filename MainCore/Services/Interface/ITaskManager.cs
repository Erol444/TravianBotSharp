using MainCore.Enums;
using MainCore.Tasks.Base;
using System;
using System.Collections.Generic;

namespace MainCore.Services.Interface
{
    public interface ITaskManager
    {
        void Remove(int index, BotTask task);

        void Clear(int index);

        BotTask GetCurrentTask(int index);

        int Count(int index);

        List<BotTask> GetList(int index);

        bool IsTaskExecuting(int index);

        AccountStatus GetAccountStatus(int index);

        void UpdateAccountStatus(int index, AccountStatus status);

        void StopCurrentTask(int index);

        void Add<T>(int accountId, bool first = false) where T : AccountBotTask;

        void Add<T>(int accountId, int villageId, bool first = false) where T : VillageBotTask;

        void ReOrder(int accountId);

        void Add<T>(int accountId, Func<T> func, bool first = false) where T : BotTask;
    }
}