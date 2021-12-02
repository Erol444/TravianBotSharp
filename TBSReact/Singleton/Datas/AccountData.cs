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
            for (int i = 0; i < AccountManager.Instance.Accounts.Count; i++)
            {
                _accounts.Add(new Account
                {
                    Id = i,
                    Name = AccountManager.Instance.Accounts[i].AccInfo.Nickname,
                    ServerUrl = AccountManager.Instance.Accounts[i].AccInfo.ServerUrl,
                });
            }
        }

        public static List<Account> Accounts => Instance._accounts;

        public static Account GetAccount(int index)
        {
            if (index < 0 || index > Accounts.Count + 1) return null;
            return Accounts[index];
        }

        public static void AddAccount(Account account)
        {
            account.Id = Accounts.Last().Id + 1;
            Accounts.Add(account);
        }

        public static bool EditAccount(int index, Account account)
        {
            if (index < 0 || index > Accounts.Count + 1) return false;
            if (account == null) return false;

            Accounts[index] = account;
            return true;
        }

        public static bool DeleteAccount(int index)
        {
            if (index < 0 || index > Accounts.Count + 1) return false;
            return Accounts.Remove(Accounts.FirstOrDefault(x => x.Id == index));
        }

        public static AccountData Instance
        {
            get { return _instance; }
        }
    }
}