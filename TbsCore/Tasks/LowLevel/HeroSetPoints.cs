using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroSetPoints : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            acc.Hero.HeroInfo = HeroParser.GetHeroInfo(htmlDoc);
            var points = acc.Hero.HeroInfo.AvaliblePoints;

            for (int i = 0; i < 4; i++)
            {
                var amount = acc.Hero.Settings.Upgrades[i];
                if (amount == 0) continue;
                var id = HeroHelper.AttributeDomId(i);
                var script = $"var attribute = document.getElementById('{id}');";
                script += "var upPoint = attribute.getElementsByClassName('pointsValueSetter')[1];";
                script += "upPoint.getElementsByTagName('a')[0].click();";

                for (int j = 0; j < amount; j++)
                {
                    // Execute the script (set point) to add 1 point
                    wb.ExecuteScript(script);
                }
                await Task.Delay(AccountHelper.Delay());
            }

            await Task.Delay(AccountHelper.Delay());
            return TaskRes.Executed;
        }
    }
}
