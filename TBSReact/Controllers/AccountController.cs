using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;
using TbsCore.Database;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Account>> GetAccounts()
        {
            var AccountInfoList = new List<Account>();

            for (int i = 0; i < AccountManager.Instance.Accounts.Count; i++)
            {
                AccountInfoList.Add(AccountManager.GetAccount(i, AccountManager.Instance.Accounts[i]));
            }

            return AccountInfoList;
        }

        [HttpGet("{index:int}")]
        public ActionResult<Account> GetAccount(int index)
        {
            if (index < 0 | index > AccountManager.Instance.Accounts.Count)
            {
                return NotFound();
            }
            return AccountManager.GetAccount(index, AccountManager.Instance.Accounts[index]);
        }

        [HttpPost]
        public ActionResult AddAccount([FromBody] Account data)
        {
            if (string.IsNullOrEmpty(data.Name) ||
                string.IsNullOrEmpty(data.ServerUrl)) return BadRequest();

            var acc = data.GetAccount();
            DbRepository.SaveAccount(acc);
            AccountManager.Instance.Accounts.Add(acc);

            return Ok();
        }

        [HttpPut("{index:int}")]
        public ActionResult EditAccount(int index, [FromBody] Account data)
        {
            if (index < 0 || index > AccountManager.Instance.Accounts.Count - 1)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(data.Name) ||
                string.IsNullOrEmpty(data.ServerUrl)) return BadRequest();

            var acc = AccountManager.Instance.Accounts[index];

            acc.AccInfo.Nickname = data.Name;
            acc.AccInfo.ServerUrl = data.ServerUrl;

            acc.Access.AllAccess.Clear();
            foreach (var access in data.Accesses)
            {
                acc.Access.AddNewAccess(new TbsCore.Models.Access.Access
                {
                    Password = access.Password,
                    Proxy = access.Proxy.Ip,
                    ProxyPort = access.Proxy.Port,
                    ProxyUsername = access.Proxy.Username,
                    ProxyPassword = access.Proxy.Password,
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