using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Models.Setting;
using TbsReact.Singleton;

namespace TbsReact.Controllers.Setting
{
    [ApiController]
    [Route("settings/activity/{indexAcc:int}")]
    public class ActivityController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Activity> GetSetting(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            var setting = new Activity
            {
                SleepTime = new Range
                {
                    Min = acc.Settings.Time.MinSleep,
                    Max = acc.Settings.Time.MaxSleep,
                },
                WorkTime = new Range
                {
                    Min = acc.Settings.Time.MinWork,
                    Max = acc.Settings.Time.MaxWork,
                }
            };

            return Ok(setting);
        }

        [HttpPut]
        public ActionResult PutSetting(int indexAcc, [FromBody] Activity setting)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            acc.Settings.Time.MinSleep = setting.SleepTime.Min;
            acc.Settings.Time.MaxSleep = setting.SleepTime.Max;
            acc.Settings.Time.MinWork = setting.WorkTime.Min;
            acc.Settings.Time.MaxWork = setting.WorkTime.Max;

            return Ok();
        }
    }
}