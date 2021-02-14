using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendTroops : BotTask
    {
        public TroopsSendModel TroopsMovement { get; set; }
        /// <summary>
        /// Other tasks (like SendDeff) can extend this task and configure amount of troops to
        /// send when getting amount of troops at home
        /// </summary>
        public Func<Account, int[], bool> TroopsCallback { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");

            TroopsCallback?.Invoke(acc, TroopsMovementParser.GetTroopsInRallyPoint(acc.Wb.Html));

            // No troops selected to be sent from this village
            if (this.TroopsMovement.Troops.Sum() == 0) return TaskRes.Executed;

            // Add number of troops to the input boxes
            await DriverHelper.WriteTroops(acc, TroopsMovement.Troops);

            // Select coordinates
            await DriverHelper.WriteCoordinates(acc, TroopsMovement.Coordinates);

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