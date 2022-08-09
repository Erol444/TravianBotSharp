using System;
using System.Timers;

namespace MainCore.Services
{
    public interface IEventManager
    {
        public event Action AccountsTableUpdate;

        public void OnAccountsTableUpdate();

        public event Action AccountStatusUpdate;

        public void OnAccountStatusUpdate();

        public event Action<int> LogUpdated;

        public void OnLogUpdated(int accountId);

        public event Action<int> TaskUpdated;

        public void OnTaskUpdated(int accountId);

        public event Action<int> TaskExecuted;

        public void OnTaskExecuted(int index);

        public event Action<int> VillagesUpdated;

        public void OnVillagesUpdated(int accountId);
    }
}