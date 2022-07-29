using System;

namespace MainCore.Services
{
    public class DatabaseEvent : IDatabaseEvent
    {
        public event Action AccountsTableUpdate;

        public void OnAccountsTableUpdate() => AccountsTableUpdate?.Invoke();

        public event Action AccountStatusUpdate;

        public void OnAccountStatusUpdate() => AccountStatusUpdate?.Invoke();

        public event Action<int> LogUpdated;

        public void OnLogUpdated(int accountId) => LogUpdated?.Invoke(accountId);

        public event Action<int> TaskUpdated;

        public void OnTaskUpdated(int accountId) => TaskUpdated?.Invoke(accountId);

        public event Action TaskExecuted;

        public void OnTaskExecuted() => TaskExecuted?.Invoke();

        public event Action<int> AccountSelected;

        public void OnAccountSelected(int accountId) => AccountSelected?.Invoke(accountId);

        public event Action<int> VillagesUpdated;

        public void OnVillagesUpdated(int accountId) => VillagesUpdated?.Invoke(accountId);

        public event Action<int> VillageSelected;

        public void OnVillageSelected(int villageId) => VillageSelected?.Invoke(villageId);

        public event Action<Type, int> TabActived;

        public void OnTabActived(Type tabType, int index) => TabActived?.Invoke(tabType, index);
    }
}