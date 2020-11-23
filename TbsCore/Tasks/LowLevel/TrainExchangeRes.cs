using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TrainExchangeRes : BotTask
    {
        public bool Great { get; set; }
        public Classificator.TroopsEnum troop { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            if (Vill == null) Vill = AccountHelper.GetMainVillage(acc);

            Classificator.BuildingEnum building = (Great == false) ? TroopsHelper.GetTroopBuilding(troop, false) : TroopsHelper.GetTroopBuilding(troop, true);

            if (!await VillageHelper.EnterBuilding(acc, Vill, building))
                return TaskRes.Executed;

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            //finding the correct "Exchange resources" button
            var exchangeResButton = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            await acc.Wb.Driver.FindElementById(exchangeResButton.Id).Click(acc);

            var distribute = acc.Wb.Html.DocumentNode.SelectNodes("//*[text()[contains(., 'Distribute remaining resources.')]]")[0];
            while (distribute.Name != "button") distribute = distribute.ParentNode;
            
            wb.FindElementById(distribute.Id).Click();

            await Task.Delay(AccountHelper.Delay());
            wb.FindElementById("npc_market_button").Click();

            return TaskRes.Executed;
        }
    }
}
