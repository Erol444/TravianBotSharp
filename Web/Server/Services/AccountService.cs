using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Database;
using TbsCore.Models.AccModels;
using TbsCore.Models.Database;
using Web.Server.Interfaces;

namespace Web.Server.Services
{
    public class AccountService : IAccountService
    {
        private List<Account> accounts;
        public AccountService()
        {
            accounts = DbRepository.GetAccounts();
        }
        public List<Account> GetAccounts() => accounts;

        public List<AccRawDTO> GetAccountsOverview()
        {
            return accounts.Select(x => new AccRawDTO() { 
                Username = x.AccInfo.Nickname,
                Server = x.AccInfo.ServerUrl,
            }).ToList();
        }

        public Account GetAccount(AccRawDTO dto)
        {
            return accounts.FirstOrDefault(x =>
                x.AccInfo.Nickname == dto.Username &&
                x.AccInfo.ServerUrl == dto.Server
            );
        }
    }
}
