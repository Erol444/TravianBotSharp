using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendDeff : BotTask
    {
        public SendDeffAmount DeffAmount { get; set; }
        public Coordinates TargetVillage { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            // Can't send deff to home village or to 0/0
            if (TargetVillage == null ||
                TargetVillage.Equals(Vill.Coordinates) ||
                TargetVillage.Equals(new Coordinates() { x = 0, y = 0 }))
            {
                return TaskRes.Executed;
            }

            if (!acc.Wb.CurrentUrl.Contains("/build.php?tt=2&id=39"))
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");
            }

            int[] troopsAtHome = TroopsMovementParser.GetTroopsInRallyPoint(acc.Wb.Html);

            int upkeepSent = 0;

            for (int i = 0; i < 10; i++)
            {
                var troop = TroopsHelper.TroopFromInt(acc, i);
                if (!TroopsHelper.IsTroopDefensive(troop) || troopsAtHome[i] == 0) continue;

                var upkeep = TroopSpeed.GetTroopUpkeep(troop);
                int sendAmount = troopsAtHome[i];

                int toSend = this.DeffAmount.Amount / upkeep;
                bool finished = false;
                if (toSend - upkeepSent < sendAmount)
                {
                    // If we have enough troops, no other tasks need to be executed
                    this.NextTask = null;
                    finished = true;
                    sendAmount = toSend;
                }

                await DriverHelper.WriteByName(acc, $"t{i + 1}", sendAmount);
                upkeepSent += sendAmount * upkeep;

                if (finished) break;
            }

            // No troops in this village
            if (upkeepSent == 0) return TaskRes.Executed;

            this.DeffAmount.Amount -= upkeepSent;
            acc.Wb.Log($"Bot will send {upkeepSent} deff (in upkeep) from {this.Vill.Name} to {this.TargetVillage}. Still needed {this.DeffAmount.Amount} deff");
            
            //select coordinates
            await DriverHelper.WriteById(acc, "xCoordInput", TargetVillage.x);
            await DriverHelper.WriteById(acc, "yCoordInput", TargetVillage.y);

            //Select reinforcement
            string script = "var radio = document.getElementsByClassName(\"radio\");for(var i = 0; i < radio.length; i++){";
            script += $"if(radio[i].value == '2') radio[i].checked = \"checked\"}}";
            await DriverHelper.ExecuteScript(acc, script);
            await DriverHelper.ClickById(acc, "btn_ok");

            // Confirm
            await DriverHelper.ClickById(acc, "btn_ok"); // Click send
            return TaskRes.Executed;
        }
    }
}
