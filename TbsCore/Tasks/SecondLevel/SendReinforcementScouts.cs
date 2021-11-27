using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;

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
            foreach(var vill in acc.Villages)
            {
                if (vill == this.Vill || stop) continue;
                base.TroopsMovement = new TroopsSendModel();
                base.TroopsMovement.Troops = ScoutsOnly(acc);
                base.TroopsMovement.MovementType = Classificator.MovementType.Reinforcement;
                base.TroopsMovement.TargetCoordinates = vill.Coordinates;
                base.SetCoordsInUrl = true;
                base.TroopsCallback = (Account _, int[] troops) =>
                {
                    for (int i = 0; i < troops.Length; i++)
                    {
                        if (TroopsData.IsTroopScout(acc, i))
                        {
                            if (this.Scouts <= troops[i]) return true;
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