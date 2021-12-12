using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsReact.Singleton;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class DriverController : ControllerBase
    {
        [HttpPost("login/{index:int}")]
        public async Task<ActionResult> Login(int index)
        {
            var account = AccountData.GetAccount(index);
            if (account == null)
            {
                return NotFound();
            }

            var acc = AccountManager.GetAccount(account);

            if (acc.Access.AllAccess.Count > 0)
            {
                AccountManager.SendMessage(account.Name, "message", $"Account {account.Name} is logging");
                await IoHelperCore.LoginAccount(acc);
                TaskManager.AddAccount(acc);
                AccountManager.SendMessage(account.Name, "message", $"Account {account.Name} logged in");

                return Ok();
            }

            return new BadRequestObjectResult("Account you are trying to login has no access defined. Please edit the account.");
        }

        [HttpPost("logout/{index:int}")]
        public ActionResult Logout(int index)
        {
            var account = AccountData.GetAccount(index);
            if (account == null)
            {
                return NotFound();
            }

            var acc = AccountManager.GetAccount(account);

            if (acc.TaskTimer != null && acc.TaskTimer.IsBotRunning() == true)
            {
                IoHelperCore.Logout(acc);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public ActionResult LoginAll()
        {
            new Thread(async () =>
            {
                var ran = new Random();
                foreach (var acc in AccountManager.Accounts)
                {
                    // If account is already running, don't login
                    if (acc.TaskTimer?.IsBotRunning() ?? false) continue;

                    _ = IoHelperCore.LoginAccount(acc);
                    await Task.Delay(AccountHelper.Delay(acc));
                }
            }).Start();

            return Ok();
        }

        [HttpPost("logout")]
        public ActionResult LogoutAll()
        {
            new Thread(() =>
            {
                foreach (var acc in AccountManager.Accounts)
                {
                    IoHelperCore.Logout(acc);
                }
            }).Start();

            return Ok();
        }

        [HttpGet("status/{index:int}")]
        public ActionResult<bool> GetStatus(int index)
        {
            var account = AccountData.GetAccount(index);
            if (account == null)
            {
                return NotFound();
            }

            var acc = AccountManager.GetAccount(account);
            if (acc.TaskTimer != null && acc.TaskTimer.IsBotRunning() == true)
            {
                return true;
            }

            return false;
        }
    }
}