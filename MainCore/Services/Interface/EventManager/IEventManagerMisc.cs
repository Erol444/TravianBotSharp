using MainCore.Models.Runtime;
using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerMisc
    {
        public event Action<int, LogMessage> LogUpdate;

        public void OnLogUpdate(int accountId, LogMessage log);

        public event Action<int> TaskExecute;

        public void OnTaskExecute(int accountId);

        public event Action<int> TaskUpdate;

        public void OnTaskUpdate(int accountId);
    }
}