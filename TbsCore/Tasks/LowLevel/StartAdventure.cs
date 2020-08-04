using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class StartAdventure : BotTask
    {
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php?t=3");

            acc.Hero.Adventures = AdventureParser.GetAdventures(htmlDoc, acc.AccInfo.ServerVersion);

            var homeVill = HeroParser.GetHeroVillageId(htmlDoc);
            if (homeVill != null) acc.Hero.HomeVillageId = homeVill ?? 0;

            if (acc.Hero.Adventures == null || acc.Hero.Adventures.Count == 0) return TaskRes.Executed;

            var adventures = acc.Hero.Adventures
                .Where(x =>
                    MapHelper.CalculateDistance(acc, x.Coordinates, MapHelper.CoordinatesFromKid(acc.Hero.HomeVillageId, acc)) <= acc.Hero.Settings.MaxDistance
                )
                .ToList();

            if (adventures.Count == 0) return TaskRes.Executed;

            var adventure = adventures.FirstOrDefault(x => x.Difficulty == 1);
            if (adventure == null) adventure = adventures.FirstOrDefault();

            acc.Hero.NextHeroSend = DateTime.Now.AddSeconds(adventure.DurationSeconds * 2);

            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/{adventure.Ref}");

                    var startButton = htmlDoc.GetElementbyId("start");
                    if (startButton == null){
                        //Hero is probably out of the village.
                        this.NextExecute = DateTime.Now.AddMinutes(10);
                        return TaskRes.Executed;
                    }
                    wb.ExecuteScript("document.getElementById('start').click()");
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    string script = $"var div = document.getElementById('{adventure.AdventureId}');";
                    script += $"div.children[0].submit();";
                    wb.ExecuteScript(script);
                    break;
            }

            return TaskRes.Executed;

        }
    }
}
