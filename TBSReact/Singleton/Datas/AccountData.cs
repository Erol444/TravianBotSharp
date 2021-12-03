using System.Collections.Generic;
using System.Linq;
using TbsReact.Models;

namespace TbsReact.Singleton
{
    public sealed class AccountData
    {
        private static readonly AccountData _instance = new();
        private List<Account> _accounts = new();

        private AccountData()
        {
            for (int i = 0; i < AccountManager.Accounts.Count; i++)
            {
                _accounts.Add(new Account
                {
                    Id = i,
                    Name = AccountManager.Accounts[i].AccInfo.Nickname,
                    ServerUrl = AccountManager.Accounts[i].AccInfo.ServerUrl,
                });
            }
        }

        public static List<Account> Accounts => Instance._accounts;

        public static Account GetAccount(int index)
        {
            return Accounts.FirstOrDefault(x => x.Id == index);
        }

        public static void AddAccount(Account account)
        {
            account.Id = Accounts.Last().Id + 1;
            Accounts.Add(account);
        }

        public static bool EditAccount(int index, Account account)
        {
            var current = Accounts.FirstOrDefault(x => x.Id == index);
            if (current == null) return false;

            current.Name = account.Name;
            current.ServerUrl = account.ServerUrl;
            return true;
        }

        public static bool DeleteAccount(int index)
        {
            return Accounts.Remove(Accounts.FirstOrDefault(x => x.Id == index));
        }

        public static AccountData Instance
        {
            get { return _instance; }
        }
    }
}