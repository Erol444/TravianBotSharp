using Microsoft.AspNetCore.Mvc;
using TbsReact.Singleton;

namespace TbsReact.Controllers
{
    public class ActivityController : ControllerBase
    {
        [HttpGet]
        [Route("api/task/{indexAcc:int}")]
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

        [HttpGet]
        [Route("logger/{indexAcc:int}")]
        public ActionResult GetLog(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(LogManager.GetLogData(account.Name));
        }
    }
}