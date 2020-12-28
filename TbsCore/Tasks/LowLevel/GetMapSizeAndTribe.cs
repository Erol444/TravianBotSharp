using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class GetMapSizeAndTribe : BotTask
    {
        /// <summary>
        /// To get tribe in T4.5, in dorf1 you can parse "<div id="resourceFieldContainer" class="resourceField7 tribe1">"
        /// 
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=2&z=1");

            // TODO: if there is no rally point, navigate to some other village.
            // If there's no rally point on entire account, build one.
            var yStr = acc.Wb.Html.GetElementbyId("yCoordInput").GetAttributeValue("value", "");
            var y = (int)Parser.RemoveNonNumeric(yStr);
            acc.AccInfo.MapSize = Math.Abs(y);

            var unitImg = acc.Wb.Html.DocumentNode.Descendants("img").First(x => x.HasClass("unit"));
            var unitInt = Parser.RemoveNonNumeric(unitImg.GetClasses().First(x => x != "unit"));
            int tribeInt = (int)(unitInt / 10);
            // ++ since the first element in Classificator.TribeEnum is Any, second is Romans.
            tribeInt++;
            acc.AccInfo.Tribe = (Classificator.TribeEnum)tribeInt;

            return TaskRes.Executed;
        }
    }
}