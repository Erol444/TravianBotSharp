using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class NPC : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;
            var market = vill.Build.Buildings.FirstOrDefault(x => x.Type == TravBotSharp.Files.Helpers.Classificator.BuildingEnum.Marketplace);
            if (market == null) return TaskRes.Executed;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?t=0&id={market.Id}");

            var npcMerchant = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            wb.ExecuteScript($"document.getElementById('{npcButton.GetAttributeValue("id", "")}').click()"); //Excgabge resources button

            await Task.Delay(AccountHelper.Delay() * 2);

            htmlDoc.LoadHtml(wb.PageSource);

            var resSum = Parser.RemoveNonNumeric(htmlDoc.GetElementbyId("remain").InnerText);
            var targetRes = MarketHelper.NpcTargetResources(vill, resSum);

            if (!vill.Market.Npc.NpcIfOverflow && MarketHelper.NpcWillOverflow(vill, targetRes))
            {
                return TaskRes.Executed;
            }
            for (int i = 0; i < 4; i++)
            {
                wb.ExecuteScript($"document.getElementById('m2[{i}]').value='{targetRes[i]}'");
                await Task.Delay(100);
            }

            var submit = htmlDoc.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();

            wb.ExecuteScript($"document.getElementById('{distribute.Id}').click()"); //Distribute resources button

            await Task.Delay(AccountHelper.Delay());
            wb.ExecuteScript($"document.getElementById('npc_market_button').click()"); //Exchange resources button
            return TaskRes.Executed;
        }
    }
}
