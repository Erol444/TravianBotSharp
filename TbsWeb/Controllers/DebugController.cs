using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using System.Collections.Generic;
using System.Linq;

using TbsCore.Models.Logging;

using TbsWeb.Singleton;
using TbsWeb.Models.Accounts;

namespace TbsWeb.Controllers
{
    [Route("debug")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        public static IHubContext<LogHub> _logHubContext;
        public static IHubContext<TaskHub> _taskHubContext;

        [HttpGet("log/{index:int}")]
        public ActionResult<string> GetLogData(int index)
        {
            if (index < 0 | index > AccountManager.Instance.Accounts.Count)
            {
                return NotFound();
            }

            return SerilogSingleton.LogOutput.GetLog(AccountManager.Instance.Accounts[index].AccInfo.Nickname);
        }

        static public List<TaskInfo> GetTasks(int index)
        {
            if (index < 0 || index > AccountManager.Instance.Accounts.Count - 1)
            {
                return null;
            }

            var acc = AccountManager.Instance.Accounts[index];
            var result = new List<TaskInfo>();

            if (acc.Tasks == null) return null;

            foreach (var task in acc.Tasks.ToList())
            {
                result.Add(new TaskInfo
                {
                    name = task.ToString().Split('.').Last(),
                    village = task.Vill?.Name ?? "/",
                    priority = task.Priority.ToString(),
                    stage = task.Stage.ToString(),
                    executeAt = task.ExecuteAt.ToString(),
                });
            }

            return result;
        }
    }
}