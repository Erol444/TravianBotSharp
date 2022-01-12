using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsReact.Models;
using TbsReact.Singleton;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accounts/{indexAcc:int}/settings/chrome")]
    public class ChromeSettingController : ControllerBase
    {
        [HttpGet]
        public ActionResult<ChromeSetting> GetSetting(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            var setting = new ChromeSetting
            {
                SleepTime = new Delay
                {
                    Min = acc.Settings.Time.MinSleep,
                    Max = acc.Settings.Time.MaxSleep,
                },
                WorkTime = new Delay
                {
                    Min = acc.Settings.Time.MinWork,
                    Max = acc.Settings.Time.MaxWork,
                },
                Click = new Delay
                {
                    Min = acc.Settings.DelayClickingMin,
                    Max = acc.Settings.DelayClickingMax,
                },
                DisableImages = acc.Settings.DisableImages,
                AutoClose = acc.Settings.AutoCloseDriver
            };

            return Ok(setting);
        }

        [HttpPatch]
        public ActionResult Logout(int indexAcc, [FromBody] ChromeSetting setting)
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
            acc.Settings.DelayClickingMin = setting.Click.Min;
            acc.Settings.DelayClickingMax = setting.Click.Max;
            acc.Settings.DisableImages = setting.DisableImages;
            acc.Settings.AutoCloseDriver = setting.AutoClose;

            return Ok();
        }
    }
}