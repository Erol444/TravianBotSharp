using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsReact.Singleton;

namespace TbsReact.Controllers
{
    [ApiController]
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

        [Route("status/{index:int}")]
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