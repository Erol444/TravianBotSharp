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
            var acc = AccountData.GetAccount(index);
            if (acc == null)
            {
                return NotFound();
            }
            return acc;
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
            AccountManager.Accounts.Add(acc);
            AccountData.AddAccount(account);

            return Ok();
        }

        [HttpPatch("{index:int}")]
        public ActionResult EditAccount(int index, [FromBody] NewAccount data)
        {
            var account = data.Account;
            var accesses = data.Accesses;

            if (string.IsNullOrEmpty(account.Name) ||
                string.IsNullOrEmpty(account.ServerUrl))
            {
                return BadRequest();
            }
            var accountOld = AccountData.GetAccount(index);
            if (accountOld == null)
            {
                return NotFound();
            }

            var acc = AccountManager.GetAccount(accountOld);

            AccountData.EditAccount(index, account);

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
            var account = AccountData.GetAccount(index);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            DbRepository.RemoveAccount(acc);
            AccountManager.Accounts.Remove(acc);
            AccountData.DeleteAccount(index);

            return Ok();
        }
    }
}