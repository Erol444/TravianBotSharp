using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class NPC : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=0"))
                return TaskRes.Executed;

            var npcMerchant = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            // Exchange resources button
            await DriverHelper.ClickById(acc, npcButton.Id);

            var resSum = Parser.RemoveNonNumeric(acc.Wb.Html.GetElementbyId("remain").InnerText);
            var targetRes = MarketHelper.NpcTargetResources(Vill, resSum);

            if (!Vill.Market.Npc.NpcIfOverflow && MarketHelper.NpcWillOverflow(Vill, targetRes))
            {
                return TaskRes.Executed;
            }
            for (int i = 0; i < 4; i++)
            {
                //await acc.Wb.Driver.FindElementById($"m2[{i}]").Write(targetRes[i]);
                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.T4_4:
                        await DriverHelper.ExecuteScript(acc, $"document.getElementById('m2[{i}]').value='{targetRes[i]}'");
                        break;
                    case Classificator.ServerVersionEnum.T4_5:
                        await DriverHelper.ExecuteScript(acc, $"document.getElementsByName('desired{i}')[0].value='{targetRes[i]}'");
                        break;
                }
            }

            var submit = acc.Wb.Html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();

            await DriverHelper.ExecuteScript(acc, $"document.getElementById('{distribute.Id}').click()");
            wb.ExecuteScript($"document.getElementById('npc_market_button').click()"); //Exchange resources button
            return TaskRes.Executed;
        }
    }
}
