using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Helpers;
using TbsCore.Parsers;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Tasks.SecondLevel
{
    public class TTWarsAddNatarsToFL : CheckProfile
    {
        public int MinPop { get; set; } = 1;
        public int MaxPop { get; set; } = 9999;
        public FarmList FL { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            base.UserId = 1;
            await base.Execute(acc);

            int addedFarms = 0;
            base.Profile.Villages = base.Profile.Villages
                .OrderBy(x => x.Coordinates.CalculateDistance(acc, this.Vill.Coordinates))
                //.Where(vill => !acc.Farming.FL.Any(x => x.Farms.Any(farm => farm.Coordinates.Equals(vill.Coordinates))))
                .ToList();
            foreach (var vill in base.Profile.Villages)
            {
                // If this farm already exists on some FL
                if (acc.Farming.FL.Any(x => x.Farms.Any(farm => farm.Coordinates.Equals(vill.Coordinates)))) continue;

                if (MinPop < vill.Population && vill.Population < MaxPop)
                {
                    acc.Tasks.Add(new AddFarm()
                    {
                        ExecuteAt = DateTime.Now.AddHours(-1),
                        Farm = new Models.VillageModels.Farm()
                        {
                            Troops = new int[] { 100 },
                            Coords = vill.Coordinates,
                        },
                        FarmListId = this.FL.Id,
                    });

                    addedFarms++;
                    if (FL.NumOfFarms + addedFarms >= 100) break; //no more slots FL slots!
                }
            }
            return TaskRes.Executed;
        }
    }
}