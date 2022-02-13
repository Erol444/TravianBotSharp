using Microsoft.AspNetCore.Mvc;
using TbsReact.Models.Setting;
using TbsReact.Singleton;

namespace TbsReact.Controllers.Setting
{
    [ApiController]
    [Route("settings/quest/{indexAcc:int}")]
    public class QuestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Quest> GetSetting(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            var setting = new Quest
            {
                Beginner = acc.Quests.ClaimBeginnerQuests,
                Daily = acc.Quests.ClaimDailyQuests,
                VillageId = acc.Quests.VillToClaim,
            };

            return Ok(setting);
        }

        [HttpPut]
        public ActionResult PutSetting(int indexAcc, [FromBody] Quest setting)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            acc.Quests.ClaimBeginnerQuests = setting.Beginner;
            acc.Quests.ClaimDailyQuests = setting.Daily;
            acc.Quests.VillToClaim = setting.VillageId;

            return Ok();
        }
    }
}