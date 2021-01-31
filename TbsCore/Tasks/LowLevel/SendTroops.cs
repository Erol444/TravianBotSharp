using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendTroops : BotTask
    {
        public TroopsMovement TroopsMovement { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");

            //add number of troops to the input boxes
            for (int i = 0; i < TroopsMovement.Troops.Length; i++)
            {
                if (TroopsMovement.Troops[i] == 0) continue;
                await DriverHelper.WriteByName(acc, $"t{i + 1}", TroopsMovement.Troops[i]);
            }

            //select coordinates
            await DriverHelper.WriteById(acc, "xCoordInput", TroopsMovement.Coordinates.x);
            await DriverHelper.WriteById(acc, "yCoordInput", TroopsMovement.Coordinates.y);
            //Select type of troop sending

            string script = $"Array.from(document.getElementsByName('c')).find(x=>x.value=={(int)TroopsMovement.MovementType}).checked=true;";
            await DriverHelper.ExecuteScript(acc, script);

            //Click on "Send" button
            await DriverHelper.ClickById(acc, "btn_ok");

            await Task.Delay(AccountHelper.Delay());

            // Select catapult targets
            if (this.TroopsMovement.Target1 != Classificator.BuildingEnum.Site)
                await DriverHelper.SelectIndexByName(acc, "ctar1", (int)this.TroopsMovement.Target1);
            if (this.TroopsMovement.Target2 != Classificator.BuildingEnum.Site)
                await DriverHelper.SelectIndexByName(acc, "ctar2", (int)this.TroopsMovement.Target2);

            // Scout type
            if (this.TroopsMovement.ScoutType != ScoutEnum.None)
            {
                string scout = $"Array.from(document.getElementsByName('spy')).find(x=>x.value=={(int)TroopsMovement.ScoutType}).checked=true;";
                await DriverHelper.ExecuteScript(acc, scout);
            }

            //Click on "Send" button
            await DriverHelper.ClickById(acc, "btn_ok");

            return TaskRes.Executed;
        }
    }
}
