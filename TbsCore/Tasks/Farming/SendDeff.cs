using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsModels;
using TbsCore.TravianData;

namespace TbsCore.Tasks.Farming
{
    public class SendDeff : SendTroops
    {
        public SendDeffAmount DeffAmount { get; set; }
        public Coordinates TargetVillage { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            // Can't send deff to home village or to 0/0
            if (TargetVillage == null ||
                TargetVillage.Equals(Vill.Coordinates) ||
                TargetVillage.Equals(new Coordinates(0, 0) { x = 0, y = 0 }))
            {
                return TaskRes.Executed;
            }

            base.TroopsMovement = new TroopsSendModel()
            {
                TargetCoordinates = TargetVillage,
                MovementType = Classificator.MovementType.Reinforcement,
                // Bot will configure amount of troops to be sent when it parses
                // the amount of troops available at home
                Troops = new int[10],
            };

            base.TroopsCallback = TroopsCountRecieved;

            await base.Execute(acc);

            return TaskRes.Executed;
        }

        public bool TroopsCountRecieved(Account acc, TroopsBase troops)
        {
            int upkeepSent = 0;

            for (int i = 0; i < 10; i++)
            {
                var troop = TroopsHelper.TroopFromInt(acc, i);
                if (!TroopsData.IsTroopDefensive(troop) || troops.Troops[i] == 0) continue;

                var upkeep = TroopsData.GetTroopUpkeep(troop);
                int sendAmount = troops.Troops[i];

                int toSend = this.DeffAmount.Amount / upkeep;
                bool finished = false;
                if (toSend - upkeepSent < sendAmount)
                {
                    // If we have enough troops, no other tasks need to be executed
                    this.NextTask = null;
                    finished = true;
                    sendAmount = toSend;
                }

                base.TroopsMovement.Troops[i] = sendAmount;

                upkeepSent += sendAmount * upkeep;

                if (finished) break;
            }

            this.DeffAmount.Amount -= upkeepSent;
            if (this.DeffAmount.Amount > 0)
            {
                acc.Logger.Warning($"Bot will send {upkeepSent} deff (in upkeep) from {this.Vill.Name} to {this.TargetVillage}. Still needed {this.DeffAmount.Amount} deff");
            }
            else
            {
                acc.Logger.Information($"Bot will send {upkeepSent} deff (in upkeep) from {this.Vill.Name} to {this.TargetVillage}.");
            }

            return true;
        }
    }
}