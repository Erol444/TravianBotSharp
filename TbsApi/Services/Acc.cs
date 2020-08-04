using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsApi.Interfaces;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TbsApi.Services
{
    public class Acc : IAcc
    {
        internal List<Account> accounts;
        public Acc()
        {
            accounts = IoHelperCore.ReadAccounts();
        }

        public List<Account> GetAccounts()
        {
            return accounts;
        }
    }
}
