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
        public TroopsMovement TroopsMovement { get; set; }
        public Func<Account, int[], bool> TroopsCallback { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");

            TroopsCallback?.Invoke(acc, TroopsMovementParser.GetTroopsInRallyPoint(acc.Wb.Html));

            // No troops selected to be sent from this village
            if (this.TroopsMovement.Troops.Sum() == 0) return TaskRes.Executed;

            //add number of troops to the input boxes
            for (int i = 0; i < TroopsMovement.Troops.Length; i++)
            {
                if (TroopsMovement.Troops[i] == 0) continue;
                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.T4_4:
                        await DriverHelper.WriteByName(acc, $"t{i + 1}", TroopsMovement.Troops[i]);
                        break;

                    case Classificator.ServerVersionEnum.T4_5:
                        await DriverHelper.WriteByName(acc, $"troops[0][t{i + 1}]", TroopsMovement.Troops[i]);
                        break;
                }
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