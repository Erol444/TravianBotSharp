using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.Sim
{
    public class NPC : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            StopFlag = false;
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            if (!await NavigationHelper.ToMarketplace(acc, Vill, NavigationHelper.MarketplaceTab.Managenment))
                return TaskRes.Executed;

            var npcMerchant = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            // Exchange resources button
            await DriverHelper.ClickById(acc, npcButton.Id);

            //wait npc form show
            var timeout = DateTime.Now.AddSeconds(10);

            HtmlNode remainRes = null;
            do
            {
                await Task.Delay(1000);

                acc.Wb.UpdateHtml();
                remainRes = acc.Wb.Html.GetElementbyId("remain");

                if (timeout < DateTime.Now)
                {
                    acc.Logger.Warning($"NPC in village {Vill.Name} is time out. Retry after 3 mins");
                    this.NextExecute = DateTime.Now.AddMinutes(3);
                    return TaskRes.Executed;
                }
            }
            while (remainRes == null);

            var resSum = Parser.RemoveNonNumeric(remainRes.InnerText);
            var targetRes = MarketHelper.NpcTargetResources(Vill, resSum);

            if (!Vill.Market.Npc.NpcIfOverflow && MarketHelper.NpcWillOverflow(Vill, targetRes))
            {
                acc.Logger.Warning($"NPC in village {Vill.Name} will be overflow. Stop NPC");
                return TaskRes.Executed;
            }
            for (int i = 0; i < 4; i++)
            {
                //await acc.Wb.Driver.FindElementById($"m2[{i}]").Write(targetRes[i]);
                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.TTwars:
                        await DriverHelper.ExecuteScript(acc, $"document.getElementById('m2[{i}]').value='{targetRes[i]}'");
                        break;

                    case Classificator.ServerVersionEnum.T4_5:
                        await DriverHelper.ExecuteScript(acc, $"document.getElementsByName('desired{i}')[0].value='{targetRes[i]}'");
                        break;
                }
            }

            var submit = acc.Wb.Html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();

            await DriverHelper.ClickById(acc, distribute.Id);
            await DriverHelper.ClickById(acc, "npc_market_button");

            return TaskRes.Executed;
        }
    }
}