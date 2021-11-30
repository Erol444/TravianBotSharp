using TbsCore.Database;
using TbsCore.Helpers;

using TbsReact.Models;

namespace TbsReact.Singleton

{
    public sealed class AccountManager
    {
        private static readonly AccountManager instance = new();

        private List<TbsCore.Models.AccModels.Account> accounts = new();

        public List<TbsCore.Models.AccModels.Account> Accounts
        {
            get { return accounts; }
            set { accounts = value; }
        }

        static AccountManager()
        {
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

        public static Account GetAccount(int index, TbsCore.Models.AccModels.Account acc)
        {
            var accesses = new List<Access>();

            for (int i = 0; i < acc.Access.AllAccess.Count; i++)
            {
                var access = acc.Access.AllAccess[i];
                accesses.Add(new Access
                {
                    Id = i,
                    Password = access.Password,
                    Proxy = new Proxy
                    {
                        Ip = access.Proxy,
                        Port = access.ProxyPort,
                        Username = access.ProxyUsername,
                        Password = access.Password,
                        OK = access.Ok
                    }
                });
            }
            return new Account
            {
                Id = index,
                Name = acc.AccInfo.Nickname,
                ServerUrl = acc.AccInfo.ServerUrl,
                Accesses = accesses
            };
        }

        private void LoadAccounts()
        {
            // For migration purposes only! Remove after few versions
            if (IoHelperCore.AccountsTxtExists() && !IoHelperCore.SQLiteExists())
            {
                DbRepository.SyncAccountsTxt();
                File.Delete(IoHelperCore.AccountsPath);
            }

            accounts = DbRepository.GetAccounts();

            accounts.ForEach(x => ObjectHelper.FixAccObj(x, x));
        }

        public void SaveAccounts()
        {
            IoHelperCore.SaveAccounts(accounts, true);
        }
    }
}