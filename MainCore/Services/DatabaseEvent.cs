using System;

namespace MainCore.Services
{
    public class DatabaseEvent : IDatabaseEvent
    {
        #region Accounts table update

        public event Action AccountsTableUpdate;

        public void OnAccountsTableUpdate()
        {
            AccountsTableUpdate?.Invoke();
        }

        #endregion Accounts table update

        #region Account status update

        public event Action AccountStatusUpdate;

        public void OnAccountStatusUpdate()
        {
            AccountStatusUpdate?.Invoke();
        }

        #endregion Account status update

        #region Log update

        public event Action<int> LogUpdated;

        public void OnLogUpdated(int accountId)
        {
            LogUpdated?.Invoke(accountId);
        }

        #endregion Log update

        #region Task update

        public event Action<int> TaskUpdated;

        public void OnTaskUpdated(int accountId)
        {
            TaskUpdated?.Invoke(accountId);
        }

        #endregion Task update

        #region Task execute

        public event Action TaskExecuted;

        public void OnTaskExecuted()
        {
            TaskExecuted?.Invoke();
        }

        #endregion Task execute

        #region Account selected

        public event Action<int> AccountSelected;

        public void OnAccountSelected(int accountId)
        {
            AccountSelected?.Invoke(accountId);
        }

        #endregion Account selected

        #region Villages table update

        public event Action<int> VillagesUpdated;

        public void OnVillagesUpdated(int accountId)
        {
            VillagesUpdated?.Invoke(accountId);
        }

        #endregion Villages table update

        #region Village selected

        public event Action<int> VillageSelected;

        public void OnVillageSelected(int villageId)
        {
            VillageSelected?.Invoke(villageId);
        }

        #endregion Village selected
    }
}