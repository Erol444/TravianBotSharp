using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;
using TbsCore.Extensions;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class NPC : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace))
                return TaskRes.Executed;

            var npcMerchant = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            wb.FindElementById(npcButton.Id).Click();

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
                await acc.Wb.Driver.FindElementById($"m2[{i}]").Write(targetRes[i]);
            }

            var submit = acc.Wb.Html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();

            await acc.Wb.Driver.FindElementById(distribute.Id).Click(acc);
            acc.Wb.Driver.FindElementById("npc_market_button").Click();
            return TaskRes.Executed;
        }
    }
}
