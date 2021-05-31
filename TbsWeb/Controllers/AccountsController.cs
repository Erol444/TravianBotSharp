using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TbsCore.Database;
using TbsCore.Models.AccModels;
using TbsCore.Models.Access;

using TbsWeb.Singleton;
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

        [HttpGet("{index:int}")]
        public ActionResult<AccountInfo> GetAccount(int index)
        {
            if (index < 0 | index > AccountManager.Instance.Accounts.Count)
            {
                return NotFound();
            }
            return new AccountInfo(AccountManager.Instance.Accounts[index]);
        }

        [HttpPost]
        public ActionResult AddAccount([FromBody] AccountInfo data)
        {
            if (string.IsNullOrEmpty(data.Username) ||
                string.IsNullOrEmpty(data.ServerUrl)) return BadRequest();

            var acc = data.GetAccount();
            DbRepository.SaveAccount(acc);
            AccountManager.Instance.Accounts.Add(acc);

            return Ok();
        }

        [HttpPut("{index:int}")]
        public ActionResult EditAccount(int index, [FromBody] AccountInfo data)
        {
            if (index < 0 || index > AccountManager.Instance.Accounts.Count - 1)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(data.Username) ||
                string.IsNullOrEmpty(data.ServerUrl)) return BadRequest();

            var acc = AccountManager.Instance.Accounts[index];

            acc.AccInfo.Nickname = data.Username;
            acc.AccInfo.ServerUrl = data.ServerUrl;

            acc.Access.AllAccess.Clear();
            foreach (var access in data.Accesses)
            {
                acc.Access.AddNewAccess(new Access()
                {
                    Password = access.Password,
                    Proxy = access.Proxy,
                    ProxyPort = access.ProxyPort,
                    ProxyUsername = access.ProxyUsername,
                    ProxyPassword = access.ProxyPassword,
                });
            }

            DbRepository.SaveAccount(acc);

            return Ok();
        }

        [HttpDelete("{index:int}")]
        public ActionResult DeleteAccount(int index)
        {
            if (index < 0 || index > AccountManager.Instance.Accounts.Count - 1)
            {
                return NotFound();
            }

            DbRepository.RemoveAccount(AccountManager.Instance.Accounts[index]);
            AccountManager.Instance.Accounts.RemoveAt(index);

            return Ok();
        }
    }
}