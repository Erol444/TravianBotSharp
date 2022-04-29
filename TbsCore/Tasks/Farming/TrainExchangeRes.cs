using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.TravianData;

namespace TbsCore.Tasks.Farming
{
    public class TrainExchangeRes : BotTask
    {
        public bool Great { get; set; }
        public Classificator.TroopsEnum troop { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (Vill == null) Vill = AccountHelper.GetMainVillage(acc);
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }
            var building = TroopsData.GetTroopBuilding(troop, Great);
            if (!await NavigationHelper.EnterBuilding(acc, Vill, building))
                return TaskRes.Executed;

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            //finding the correct "Exchange resources" button
            var exchangeResButton = troopNode.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));
            await DriverHelper.ClickById(acc, exchangeResButton.Id);

            var distribute = acc.Wb.Html.DocumentNode.SelectNodes("//*[text()[contains(., 'Distribute remaining resources.')]]")[0];
            while (distribute.Name != "button") distribute = distribute.ParentNode;
            await DriverHelper.ClickById(acc, distribute.Id);

            await Task.Delay(AccountHelper.Delay(acc));
            await DriverHelper.ClickById(acc, "npc_market_button");

            return TaskRes.Executed;
        }
    }
}