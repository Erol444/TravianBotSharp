using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;

namespace TbsCore.Tasks.Farming
{
    public class AddFarm : BotTask
    {
        public int FarmListId { get; set; }
        public Farm Farm { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            await NavigationHelper.ToRallyPoint(acc, Vill, NavigationHelper.RallyPointTab.Farmlist);

            // Show "Add raid" popup
            await DriverHelper.ExecuteScript(acc, $"Travian.Game.RaidList.addSlot({this.FarmListId},'','','rallyPoint');");

            // Input coordinates
            await DriverHelper.WriteCoordinates(acc, Farm.Coords);

            // Input troops
            for (int i = 0; i < Farm.Troops.Length; i++)
            {
                if (Farm.Troops[i] == 0) continue;
                await DriverHelper.WriteById(acc, $"t{i + 1}", Farm.Troops[i]);
            }

            await Task.Delay(AccountHelper.Delay(acc));

            // Click "save"
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    acc.Wb.ExecuteScript("Travian.Game.RaidList.saveSlot(getSelectedListId(), $('edit_form').toQueryString().parseQueryString(), true);");
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    await DriverHelper.ClickById(acc, "save");
                    break;
            }

            return TaskRes.Executed;
        }
    }
}