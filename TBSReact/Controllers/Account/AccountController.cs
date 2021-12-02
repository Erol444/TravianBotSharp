using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;
using TbsCore.Database;
using System;

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
            for (int i = 0; i < AccountData.Accounts.Count; i++)
            {
                AccountInfoList.Add(AccountData.Accounts[i]);
            }

            return AccountInfoList;
        }

        [HttpGet("{index:int}")]
        public ActionResult<Account> GetAccount(int index)
        {
            if (index < 0 || index > AccountManager.Instance.Accounts.Count - 1)
            {
                return NotFound();
            }
            return AccountData.GetAccount(index);
        }

        [HttpPost]
        public ActionResult AddAccount([FromBody] NewAccount data)
        {
            var account = data.Account;
            var accesses = data.Accesses;
            if (string.IsNullOrEmpty(account.Name)
                || string.IsNullOrEmpty(account.ServerUrl))
            {
                return BadRequest();
            }
            var acc = account.GetAccount(accesses);
            DbRepository.SaveAccount(acc);
            AccountManager.Instance.Accounts.Add(acc);
            AccountData.AddAccount(account);

            return Ok();
        }

        [HttpPatch("{index:int}")]
        public ActionResult EditAccount(int index, [FromBody] NewAccount data)
        {
            if (index < 0 || index > AccountManager.Instance.Accounts.Count - 1)
            {
                return NotFound();
            }
            var account = data.Account;
            var accesses = data.Accesses;

            if (string.IsNullOrEmpty(account.Name) ||
                string.IsNullOrEmpty(account.ServerUrl))
            {
                return BadRequest();
            }

            var acc = AccountManager.Instance.Accounts[index];

            acc.AccInfo.Nickname = account.Name;
            acc.AccInfo.ServerUrl = account.ServerUrl;

            acc.Access.AllAccess.Clear();
            foreach (var access in accesses)
            {
                acc.Access.AllAccess.Add(new TbsCore.Models.Access.Access
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
            AccountData.DeleteAccount(index);

            return Ok();
        }
    }
}