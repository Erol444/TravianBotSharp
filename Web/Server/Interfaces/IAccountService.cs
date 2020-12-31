using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.Database;

namespace Web.Server.Interfaces
{
    public interface IAccountService
    {
        public List<Account> GetAccounts();
        public Account GetAccount(AccRawDTO dto);
        public List<AccRawDTO> GetAccountsOverview();
    }
}
