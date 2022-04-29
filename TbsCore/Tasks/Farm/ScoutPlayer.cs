using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Tasks.LowLevel;
using TbsCore.Tasks.Update;
using TbsCore.TravianData;

namespace TbsCore.Tasks.SecondLevel
{
    public class ScoutPlayer : CheckProfile
    {
        public int MinPop { get; set; } = 100;
        public int MaxPop { get; set; } = 1000;
        public int ScoutNum { get; set; } = 1;

        public override async Task<TaskRes> Execute(Account acc)
        {
            await base.Execute(acc);

            foreach (var vill in base.Profile.Villages)
            {
                if (MinPop < vill.Population && vill.Population < MaxPop)
                {
                    // If Natars && capital, don't send scouts (you can't)
                    if ((base.UserId == 1 && vill.Capital)) continue;

                    var sendTask = new SendTroops()
                    {
                        TroopsMovement = new Models.SendTroopsModels.TroopsSendModel()
                        {
                            ScoutType = Models.SendTroopsModels.ScoutEnum.Resources,
                            MovementType = Classificator.MovementType.Raid,
                            TargetCoordinates = vill.Coordinates,
                            Troops = GenerateScoutTroops(acc),
                        },
                        Vill = this.Vill,
                    };
                    await sendTask.Execute(acc);

                    acc.Tasks.Add(new ReadFarmScoutReport()
                    {
                        Coordinates = vill.Coordinates,
                        ExecuteAt = DateTime.Now.Add(sendTask.Arrival),
                        Vill = this.Vill
                    });
                }
            }
            return TaskRes.Executed;
        }

        public int[] GenerateScoutTroops(Account acc)
        {
            int[] ret = new int[11];
            var scout = (int)TroopsData.TribeScout(acc.AccInfo.Tribe);
            ret[scout % 10 - 1] = this.ScoutNum;
            return ret;
        }
    }
}