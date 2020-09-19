using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class NPC : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var market = Vill.Build.Buildings.FirstOrDefault(x => x.Type == TravBotSharp.Files.Helpers.Classificator.BuildingEnum.Marketplace);
            if (market == null) return TaskRes.Executed;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?t=0&id={market.Id}");

            var npcMerchant = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            wb.ExecuteScript($"document.getElementById('{npcButton.GetAttributeValue("id", "")}').click()"); //Excgabge resources button

            await Task.Delay(AccountHelper.Delay() * 2);

            acc.Wb.Html.LoadHtml(wb.PageSource);

            var resSum = Parser.RemoveNonNumeric(acc.Wb.Html.GetElementbyId("remain").InnerText);
            var targetRes = MarketHelper.NpcTargetResources(Vill, resSum);

            if (!Vill.Market.Npc.NpcIfOverflow && MarketHelper.NpcWillOverflow(Vill, targetRes))
            {
                return TaskRes.Executed;
            }
            for (int i = 0; i < 4; i++)
            {
                await DriverHelper.ExecuteScript(acc, $"document.getElementById('m2[{i}]').value='{targetRes[i]}'");
            }

            var submit = acc.Wb.Html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();

            await DriverHelper.ExecuteScript(acc, $"document.getElementById('{distribute.Id}').click()");
            wb.ExecuteScript($"document.getElementById('npc_market_button').click()"); //Exchange resources button
            return TaskRes.Executed;
        }
    }
}
