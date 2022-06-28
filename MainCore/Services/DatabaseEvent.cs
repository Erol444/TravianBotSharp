namespace MainCore.Services
{
    public class DatabaseEvent
    {
        #region Accounts

        public delegate void AccountsTableUpdateDelegate();

        public AccountsTableUpdateDelegate AccountsTableUpdate;

        public void OnAccountsTableUpdate()
        {
            AccountsTableUpdate?.Invoke();
        }

        #endregion Accounts
    }
}