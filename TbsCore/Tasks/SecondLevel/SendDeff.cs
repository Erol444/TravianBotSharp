using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendDeff : SendTroops
    {
        public SendDeffAmount DeffAmount { get; set; }
        public Coordinates TargetVillage { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            // Can't send deff to home village or to 0/0
            if (TargetVillage == null ||
                TargetVillage.Equals(Vill.Coordinates) ||
                TargetVillage.Equals(new Coordinates(0, 0) {x = 0, y = 0}))
                return TaskRes.Executed;

            TroopsMovement = new TroopsSendModel
            {
                TargetCoordinates = TargetVillage,
                MovementType = Classificator.MovementType.Reinforcement,
                // Bot will configure amount of troops to be sent when it parses
                // the amount of troops available at home
                Troops = new int[10]
            };

            TroopsCallback = TroopsCountRecieved;

            await base.Execute(acc);

            return TaskRes.Executed;
        }

        public bool TroopsCountRecieved(Account acc, int[] troopsAtHome)
        {
            var upkeepSent = 0;

            for (var i = 0; i < 10; i++)
            {
                var troop = TroopsHelper.TroopFromInt(acc, i);
                if (!TroopsData.IsTroopDefensive(troop) || troopsAtHome[i] == 0) continue;

                var upkeep = TroopSpeed.GetTroopUpkeep(troop);
                var sendAmount = troopsAtHome[i];

                var toSend = DeffAmount.Amount / upkeep;
                var finished = false;
                if (toSend - upkeepSent < sendAmount)
                {
                    // If we have enough troops, no other tasks need to be executed
                    NextTask = null;
                    finished = true;
                    sendAmount = toSend;
                }

                TroopsMovement.Troops[i] = sendAmount;

                upkeepSent += sendAmount * upkeep;

                if (finished) break;
            }

            DeffAmount.Amount -= upkeepSent;
            acc.Wb.Log(
                $"Bot will send {upkeepSent} deff (in upkeep) from {Vill.Name} to {TargetVillage}. Still needed {DeffAmount.Amount} deff");

            return true;
        }
    }
}