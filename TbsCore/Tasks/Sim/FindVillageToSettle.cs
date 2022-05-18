using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.Sim
{
    public class FindVillageToSettle : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToMap(acc);

            var mainVill = AccountHelper.GetMainVillage(acc);

            var mapTiles = await MapHelper.GetMapTiles(acc, mainVill.Coordinates);

            Coordinates closesCoords = GetClosestCoordinates(acc, mapTiles);
            if (closesCoords == null) return TaskRes.Retry;

            acc.NewVillages.Locations.Add(new NewVillage()
            {
                Coordinates = closesCoords,
                Name = NewVillageHelper.GenerateName(acc),
            });

            return TaskRes.Executed;
        }

        private Coordinates GetClosestCoordinates(Account acc, List<MapTile> tiles)
        {
            var mainVill = AccountHelper.GetMainVillage(acc);
            var closesCoords = new Coordinates();
            var closest = 1000.0;
            foreach (var tile in tiles)
            {
                if (tile.Title == null || !tile.Title.StartsWith("{k.vt}")) continue;

                // Check if village type meets criteria
                if (acc.NewVillages.Types.Count != 0)
                {
                    var num = (int)Parser.RemoveNonNumeric(tile.Title.Split('f')[1]);
                    var type = (Classificator.VillTypeEnum)(num);
                    if (!acc.NewVillages.Types.Any(x => x == type)) continue;
                }

                var distance = mainVill.Coordinates.CalculateDistance(acc, tile.Coordinates);
                if (distance < closest)
                {
                    closest = distance;
                    closesCoords = tile.Coordinates;
                }
            }
            return closesCoords;
        }
    }
}