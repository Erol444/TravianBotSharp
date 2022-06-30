namespace MainCore.Services
{
    public class DatabaseEvent : IDatabaseEvent
    {
        #region Accounts

        public event Action AccountsTableUpdate;

        public void OnAccountsTableUpdate()
        {
            AccountsTableUpdate?.Invoke();
        }

        #endregion Accounts
    }
}