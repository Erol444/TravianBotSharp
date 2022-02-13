using Microsoft.AspNetCore.Mvc;
using TbsReact.Models.Setting;
using TbsReact.Singleton;

namespace TbsReact.Controllers.Setting
{
    [ApiController]
    [Route("settings/discordwebhook/{indexAcc:int}")]
    public class DiscordController : ControllerBase
    {
        [HttpGet]
        public ActionResult<DiscordWebhook> GetSetting(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            var setting = new DiscordWebhook
            {
                IsActive = acc.Settings.DiscordWebhook,
                IsOnlineMsg = acc.Settings.DiscordOnlineAnnouncement,
                UrlWebhook = acc.AccInfo.WebhookUrl,
            };

            return Ok(setting);
        }

        [HttpPut]
        public ActionResult PutSetting(int indexAcc, [FromBody] DiscordWebhook setting)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            acc.Settings.DiscordWebhook = setting.IsActive;
            acc.Settings.DiscordOnlineAnnouncement = setting.IsOnlineMsg;
            acc.AccInfo.WebhookUrl = setting.UrlWebhook;

            return Ok();
        }
    }
}