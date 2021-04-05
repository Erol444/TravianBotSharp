using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TravBotSharp.Files.Helpers;
using TbsWeb.Singleton;

namespace TbsWeb.Controllers
{
    [Route("driver")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        [HttpPost("login/{index:int}")]
        public async Task<ActionResult> Login(int index)
        {
            if (index < 0 | index > AccountManager.Instance.Accounts.Count)
            {
                return NotFound();
            }
            var acc = AccountManager.Instance.Accounts[index];

            if (acc.Access.AllAccess.Count > 0)
            {
                await IoHelperCore.LoginAccount(acc);
                return Ok();
            }

            return new BadRequestObjectResult("Account you are trying to login has no access defined. Please edit the account.");
        }

        [HttpPost("logout/{index:int}")]
        public ActionResult Logout(int index)
        {
            if (index < 0 | index > AccountManager.Instance.Accounts.Count)
            {
                return NotFound();
            }
            var acc = AccountManager.Instance.Accounts[index];

            if (acc.TaskTimer != null && acc.TaskTimer.IsBotRunning() == true)
            {
                IoHelperCore.Logout(acc);
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("status/{index:int}")]
        public ActionResult<bool> GetStatus(int index)
        {
            if (index < 0 | index > AccountManager.Instance.Accounts.Count)
            {
                return NotFound();
            }

            var acc = AccountManager.Instance.Accounts[index];
            if (acc.TaskTimer != null && acc.TaskTimer.IsBotRunning() == true)
            {
                return true;
            }

            return false;
        }
    }
}