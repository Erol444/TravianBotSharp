using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsModels;
using TbsCore.TravianData;

namespace TbsCore.Tasks.LowLevel
{
    public class SendRaid : SendTroops
    {
        /// <summary>
        /// Bot will only send enough troops to raid all resources availabe
        /// </summary>
        public long ResourcesAvailable { get; set; }
        public Coordinates TargetVillage { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            base.TroopsMovement = new TroopsSendModel()
            {
                TargetCoordinates = TargetVillage,
                MovementType = Classificator.MovementType.Raid,
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
            // Send 10% more troops than available resources
            this.ResourcesAvailable = (long)(this.ResourcesAvailable * 1.1);

            //long resCap = 0;

            for (int i = 0; i < 10; i++)
            {
                var troop = TroopsHelper.TroopFromInt(acc, i);
                if (TroopsData.IsTroopDefensive(troop) || troops.Troops[i] == 0) continue;

                int sendAmount = troops.Troops[i];
                if (sendAmount < 500) continue; // TODO: configurable

                var troopCap = TroopsData.TroopCapacity(troop);

                long toSend = this.ResourcesAvailable / (long)troopCap;
                if (toSend < sendAmount)
                {
                    sendAmount = (int)toSend;
                }

                base.TroopsMovement.Troops[i] = sendAmount;
                break;
                //resCap += sendAmount * troopCap;
            }

            return true;
        }
    }
}