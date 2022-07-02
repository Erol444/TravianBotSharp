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
    }
}