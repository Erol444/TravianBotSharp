using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Models.Setting;
using TbsReact.Singleton;

namespace TbsReact.Controllers.Setting
{
    [ApiController]
    [Route("api/settings/hero/{indexAcc:int}")]
    public class HeroController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Hero> GetSetting(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            var setting = new Hero
            {
                AutoAdventure = new AutoAdventure
                {
                    IsActive = acc.Hero.Settings.AutoSendToAdventure,
                    MaxDistance = acc.Hero.Settings.MaxDistance,
                    MinHealth = acc.Hero.Settings.MinHealth,
                },
                AutoPoint = new AutoPoint
                {
                    IsActive = acc.Hero.Settings.AutoSetPoints,
                    Points = acc.Hero.Settings.Upgrades,
                },
                AutoRefresh = new AutoRefresh
                {
                    IsActive = acc.Hero.Settings.AutoRefreshInfo,
                    Frequency = new Range
                    {
                        Min = acc.Hero.Settings.MinUpdate,
                        Max = acc.Hero.Settings.MaxUpdate,
                    }
                },
                AutoRevive = new AutoRevive
                {
                    IsActive = acc.Hero.Settings.AutoReviveHero,
                    VillageId = acc.Hero.HomeVillageId,
                },
            };

            return Ok(setting);
        }

        [HttpPut]
        public ActionResult PutSetting(int indexAcc, [FromBody] Hero setting)
        {
            var account = AccountData.GetAccount(indexAcc);

            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);

            acc.Hero.Settings.AutoSendToAdventure = setting.AutoAdventure.IsActive;
            acc.Hero.Settings.MaxDistance = setting.AutoAdventure.MaxDistance;
            acc.Hero.Settings.MinHealth = setting.AutoAdventure.MinHealth;

            acc.Hero.Settings.AutoSetPoints = setting.AutoPoint.IsActive;
            acc.Hero.Settings.Upgrades = setting.AutoPoint.Points;

            acc.Hero.Settings.AutoRefreshInfo = setting.AutoRefresh.IsActive;
            acc.Hero.Settings.MinUpdate = setting.AutoRefresh.Frequency.Min;
            acc.Hero.Settings.MaxUpdate = setting.AutoRefresh.Frequency.Max;

            acc.Hero.Settings.AutoReviveHero = setting.AutoRevive.IsActive;
            acc.Hero.HomeVillageId = setting.AutoRevive.VillageId;

            return Ok();
        }
    }
}