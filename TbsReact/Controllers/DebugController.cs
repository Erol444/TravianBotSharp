using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accounts/{indexAcc:int}")]
    public class DebugController : ControllerBase
    {
        [HttpGet]
        [Route("log")]
        public ActionResult GetLog(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(LogManager.GetLogData(account.Name));
        }

        [HttpGet]
        [Route("task")]
        public ActionResult GetTask(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var list = TaskManager.GetTaskList(account.Name);
            if (list == null)
            {
                return Ok("null");
            }
            return Ok(list);
        }
    }
}