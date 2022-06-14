using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.Others
{
    public class HeroSetPoints : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Attributes);

            acc.Hero.HeroInfo = HeroParser.GetHeroInfo(acc.Wb.Html);
            acc.Hero.HeroArrival = DateTime.Now + HeroParser.GetHeroArrivalInfo(acc.Wb.Html);

            float sum = 0;
            for (int i = 0; i < 4; i++) sum += acc.Hero.Settings.Upgrades[i];
            if (sum == 0)
            {
                // Upgrade points were not set. Set points to default
                acc.Hero.Settings.Upgrades = new byte[4] { 2, 0, 0, 2 };
                sum = 4;
            }

            var points = acc.Hero.HeroInfo.AvaliblePoints;

            //await Driver
            return TaskRes.Executed;
        }
    }
}