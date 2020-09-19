using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroSetPoints : BotTask
    {
        private readonly string[] domId = new string[] {
            "attributepower",
            "attributeoffBonus",
            "attributedefBonus",
            "attributeproductionPoints"
        };

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            HeroHelper.ParseHeroPage(acc);
            var points = acc.Hero.HeroInfo.AvaliblePoints;

            for (int i = 0; i < 4; i++)
            {
                var amount = Math.Ceiling(acc.Hero.Settings.Upgrades[i] * points / 4.0);
                if (amount == 0) continue;

                var script = $"var attribute = document.getElementById('{domId[i]}');";
                script += "var upPoint = attribute.getElementsByClassName('pointsValueSetter')[1];";
                script += "upPoint.getElementsByTagName('a')[0].click();";

                for (int j = 0; j < (int)amount; j++)
                {
                    // Execute the script (set point) to add 1 point
                    wb.ExecuteScript(script);
                }
                await Task.Delay(AccountHelper.Delay());
            }

            wb.ExecuteScript("document.getElementById('saveHeroAttributes').click();");
            return TaskRes.Executed;
        }
    }
}
