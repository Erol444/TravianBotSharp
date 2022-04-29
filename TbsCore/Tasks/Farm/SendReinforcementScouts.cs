using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Tasks.LowLevel;
using TbsCore.TravianData;

namespace TbsCore.Tasks.SecondLevel
{
    /// <summary>
    /// Send scouts to all your villages
    /// </summary>
    public class SendReinforcementScouts : SendTroops
    {
        public int Scouts { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            if (Scouts == 0) return TaskRes.Executed;

            bool stop = false;
            foreach (var vill in acc.Villages)
            {
                if (vill == this.Vill || stop) continue;
                base.TroopsMovement = new TroopsSendModel();
                base.TroopsMovement.Troops = ScoutsOnly(acc);
                base.TroopsMovement.MovementType = Classificator.MovementType.Reinforcement;
                base.TroopsMovement.TargetCoordinates = vill.Coordinates;
                base.TroopsCallback = (Account _, TroopsBase t) =>
                {
                    for (int i = 0; i < t.Troops.Length; i++)
                    {
                        if (TroopsData.IsTroopScout(acc, i))
                        {
                            if (this.Scouts <= t.Troops[i]) return true;
                            stop = true;
                            return false;
                        }
                    }
                    return false; // Don't continue with the SendTroops
                };
                acc.Logger.Information($"Sending scouts to {vill.Name} (reinforcement)");
                await base.Execute(acc);
            }

            return TaskRes.Executed;
        }

        private int[] ScoutsOnly(Account acc)
        {
            var ret = new int[11];
            for (int i = 0; i < 10; i++)
            {
                if (TroopsData.IsTroopScout(acc, i))
                {
                    ret[i] = this.Scouts;
                    break;
                }
            }
            return ret;
        }
    }
}