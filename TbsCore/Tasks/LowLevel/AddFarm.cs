using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class AddFarm : BotTask
    {
        public int FarmListId { get; set; }
        public Farm Farm { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=99");

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

            await Task.Delay(AccountHelper.Delay());

            // Click "save"
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    wb.ExecuteScript("Travian.Game.RaidList.saveSlot(getSelectedListId(), $('edit_form').toQueryString().parseQueryString(), true);");
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    await DriverHelper.ClickById(acc, "save");
                    break;
            }

            return TaskRes.Executed;
        }
    }
}