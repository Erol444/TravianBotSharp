using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsReact.Singleton;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accounts/{index:int}")]
    public class DriverController : ControllerBase
    {
        [HttpPost("login")]
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
                await IoHelperCore.LoginAccount(acc);

                return Ok();
            }

            return new BadRequestObjectResult("Account you are trying to login has no access defined. Please edit the account.");
        }

        [HttpPost("logout")]
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

        [HttpGet("status")]
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