using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TbsCore.Database;
using TbsCore.Helpers;
using TbsReact.Hubs;
using TbsReact.Models;

namespace TbsReact.Singleton
{
    public sealed class AccountManager
    {
        private static readonly AccountManager instance = new();

        private ConcurrentDictionary<string, int> group = new();

        private IHubContext<GroupHub> _groupContext;
        private List<TbsCore.Models.AccModels.Account> accounts = new();

        public static List<TbsCore.Models.AccModels.Account> Accounts
        {
            get { return Instance.accounts; }
        }

        private AccountManager()
        {
            LoadAccounts();
        }

        public static AccountManager Instance
        {
            get
            {
                return instance;
            }
        }

        public static TbsCore.Models.AccModels.Account GetAccount(Account account)
        {
            return Accounts.FirstOrDefault(x => x.AccInfo.Nickname.Equals(account.Name) && x.AccInfo.ServerUrl.Equals(account.ServerUrl));
        }

        private void LoadAccounts()
        {
            accounts = DbRepository.GetAccounts();
            accounts.ForEach(x => ObjectHelper.FixAccObj(x, x));
        }

        public void SaveAccounts()
        {
            IoHelperCore.SaveAccounts(accounts, true);
        }

        #region Hub SignalR

        public static void SetHub(IHubContext<GroupHub> groupContext)
        {
            instance._groupContext = groupContext;
        }

        public static void ClientConnect(string key)
        {
            instance.group.AddOrUpdate(key, 1, (key, value) => value++);
        }

        public static void ClientDisconnect(string key)
        {
            instance.group.AddOrUpdate(key, 0, (key, value) => value--);
        }

        public static bool CheckGroup(string key)
        {
            instance.group.TryGetValue(key, out int value);
            return (value > 0);
        }

        public static async void SendMessage(string groupkey, string type, string message)
        {
            await Instance._groupContext.Clients.Group(groupkey).SendAsync(type, message);
        }

        #endregion Hub SignalR
    }
}