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

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=99&id=39");

            // Show "Add raid" popup
            await DriverHelper.ExecuteScript(acc, $"Travian.Game.RaidList.addSlot({this.FarmListId},'','','rallyPoint');");

            // Input coordinates
            await DriverHelper.WriteCoordinates(acc, Farm.Coords);

            // Input troops
            await DriverHelper.WriteTroops(acc, Farm.Troops, false);
            
            await Task.Delay(AccountHelper.Delay());

            //click "save"
            wb.ExecuteScript("Travian.Game.RaidList.saveSlot(getSelectedListId(), $('edit_form').toQueryString().parseQueryString(), true);");
            return TaskRes.Executed;
        }
    }
}
