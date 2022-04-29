using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Tasks.Update;

namespace TbsCore.Tasks.Farming
{
    public class TTWarsAddNatarsToNonGoldFL : CheckProfile
    {
        public int MinPop { get; set; }
        public int MaxPop { get; set; }
        public Models.VillageModels.FarmList FL { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            base.UserId = 1;
            await base.Execute(acc);

            var villages = base.Profile.Villages
                .Where(vill => MinPop < vill.Population && vill.Population < MaxPop)
                .OrderBy(x => x.Coordinates.CalculateDistance(acc, this.Vill.Coordinates))
                .ToList();

            for (int i = 0; i < villages.Count; i++)
            {
                int[] troops = new int[10];
                if (i < (int)(villages.Count / 2)) troops[2] = 400;
                else troops[4] = 200;
                FL.Targets.Add(new Models.VillageModels.Farm(troops, villages[i].Coordinates));
            }
            return TaskRes.Executed;
        }
    }
}