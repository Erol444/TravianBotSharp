using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerAccount
    {
        public event Action AccountsTableUpdate;

        public void OnAccountsUpdate();

        public event Action<int> AccountStatusUpdate;

        public void OnStatusUpdate(int accountId);
    }
}