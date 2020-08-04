using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class GetMapSize : BotTask
    {
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=2&z=1");

            var yStr = htmlDoc.GetElementbyId("yCoordInput").GetAttributeValue("value", "");
            var y = (int)Parser.RemoveNonNumeric(yStr);
            acc.AccInfo.MapSize = Math.Abs(y);
            return TaskRes.Executed;
        }
    }
}