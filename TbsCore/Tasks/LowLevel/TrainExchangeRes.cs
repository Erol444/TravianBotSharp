using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.TravianData;

namespace TbsCore.Tasks.LowLevel
{
    public class TrainExchangeRes : BotTask
    {
        public bool Great { get; set; }
        public Classificator.TroopsEnum troop { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (Vill == null) Vill = AccountHelper.GetMainVillage(acc);

            var building = TroopsData.GetTroopBuilding(troop, Great);
            if (!await NavigationHelper.EnterBuilding(acc, Vill, building))
                return TaskRes.Executed;

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            //finding the correct "Exchange resources" button
            var exchangeResButton = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));

            string script = $"document.getElementById('{exchangeResButton.GetAttributeValue("id", "")}').click()";
            await DriverHelper.ExecuteScript(acc, script);

            var distribute = acc.Wb.Html.DocumentNode.SelectNodes("//*[text()[contains(., 'Distribute remaining resources.')]]")[0];
            while (distribute.Name != "button") distribute = distribute.ParentNode;
            string distributeid = distribute.GetAttributeValue("id", "");
            acc.Wb.ExecuteScript($"document.getElementById('{distributeid}').click()"); //Distribute resources button

            await Task.Delay(AccountHelper.Delay(acc));
            acc.Wb.ExecuteScript($"document.getElementById('npc_market_button').click()"); //Exchange resources button

            return TaskRes.Executed;
        }
    }
}