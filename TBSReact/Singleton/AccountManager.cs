using System.Collections.Generic;
using System.IO;
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