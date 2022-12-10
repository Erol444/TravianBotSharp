using MainCore.Enums;
using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerAccount
    {
        public event Action AccountsTableUpdate;

        public void OnAccountsUpdate();

        public event Action<int, AccountStatus> AccountStatusUpdate;

        public void OnStatusUpdate(int accountId, AccountStatus status);
    }
}