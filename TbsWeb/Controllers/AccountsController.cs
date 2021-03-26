using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TbsWeb.Singleton;
using TbsCore.Models.AccModels;
using TbsWeb.Models.Accounts;

namespace TbsWeb.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<AccountInfo>> GetAccounts()
        {
            var AccountInfoList = new List<AccountInfo>();

            foreach (var acc in
                AccountManager.Instance.Accounts)
            {
                AccountInfoList.Add(new AccountInfo(acc));
            }

            return AccountInfoList;
        }
    }
}