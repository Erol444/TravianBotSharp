using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class CheckHeroInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php?t=1");

            acc.Hero.HeroInfo = HeroParser.GetHeroInfo(htmlDoc);
            var homeVill = HeroParser.GetHeroVillageId(htmlDoc);
            if (homeVill != null) acc.Hero.HomeVillageId = homeVill ?? 0;

            if (acc.Hero.HeroInfo.Health > acc.Hero.Settings.MinHealth && acc.Hero.Settings.AutoSendToAdventure)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new StartAdventure() { ExecuteAt = DateTime.MinValue.AddMilliseconds(1) });
            }
            return TaskRes.Executed;
        }
    }
}
