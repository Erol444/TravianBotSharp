using System.Collections.Generic;
using System.IO;
using System.Linq;
using TbsCore.Database;
using TbsCore.Helpers;

using TbsReact.Models;

namespace TbsReact.Singleton
{
    public sealed class AccountManager
    {
        private static readonly AccountManager instance = new();

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
    }
}