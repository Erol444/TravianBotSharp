using System.Collections.Generic;
using System.IO;
using TbsCore.Models.AccModels;
using TbsCore.Database;
using TbsCore.Helpers;

namespace TbsWeb.Singleton

{
    public sealed class AccountManager
    {
        private static readonly AccountManager instance = new AccountManager();

        private List<Account> accounts = new List<Account>();

        public List<Account> Accounts
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