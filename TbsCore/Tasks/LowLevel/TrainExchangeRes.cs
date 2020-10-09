using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
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

            var buildId = Vill.Build.Buildings.FirstOrDefault(x => x.Type == building);
            if (buildId == null)
            {
                //update dorf, no buildingId found?
                TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = DateTime.Now });
                Console.WriteLine($"There is no {building} in this village!");
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={buildId.Id}");

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            //finding the correct "Exchange resources" button
            var exchangeResButton = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            string script = $"document.getElementById('{exchangeResButton.GetAttributeValue("id", "")}').click()";
            await DriverHelper.ExecuteScript(acc, script);

            var distribute = acc.Wb.Html.DocumentNode.SelectNodes("//*[text()[contains(., 'Distribute remaining resources.')]]")[0];
            while (distribute.Name != "button") distribute = distribute.ParentNode;
            string distributeid = distribute.GetAttributeValue("id", "");
            wb.ExecuteScript($"document.getElementById('{distributeid}').click()"); //Distribute resources button

            await Task.Delay(AccountHelper.Delay());
            wb.ExecuteScript($"document.getElementById('npc_market_button').click()"); //Exchange resources button

            return TaskRes.Executed;
        }
    }
}
