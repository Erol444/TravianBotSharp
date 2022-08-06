using MainCore.Enums;
using MainCore.Services;
using MainCore.TravianData;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper
{
    public static class BuildingsHelper
    {
        public static int GetDorf(int index) => index < 19 ? 1 : 2;

        public static int GetDorf(BuildingEnums building) => GetDorf((int)building);

        public static List<BuildingEnums> GetCanBuild(AppDbContext context, IPlanManager planManager, int accountId, int villageId)
        {
            var result = new List<BuildingEnums>(); var tribe = context.AccountsInfo.Find(accountId).Tribe;
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                if (CanBuild(context, planManager, villageId, tribe, i))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public static bool CanBuild(AppDbContext context, IPlanManager planManager, int villageId, TribeEnums tribe, BuildingEnums building)
        {
            bool exists = (context.VillagesBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == building) is not null); //there is already a building of this type in the vill
            if (exists)
            {
                //check cranny/warehouse/grannary/trapper/GG/GW
                return building switch
                {
                    BuildingEnums.Warehouse => IsBuildingAboveLevel(context, planManager, villageId, BuildingEnums.Warehouse, 20),
                    BuildingEnums.Granary => IsBuildingAboveLevel(context, planManager, villageId, BuildingEnums.Granary, 20),
                    BuildingEnums.GreatWarehouse => IsBuildingAboveLevel(context, planManager, villageId, BuildingEnums.GreatWarehouse, 20),
                    BuildingEnums.GreatGranary => IsBuildingAboveLevel(context, planManager, villageId, BuildingEnums.GreatGranary, 20),
                    BuildingEnums.Trapper => IsBuildingAboveLevel(context, planManager, villageId, BuildingEnums.Trapper, 20),
                    BuildingEnums.Cranny => IsBuildingAboveLevel(context, planManager, villageId, BuildingEnums.Cranny, 10),
                    _ => false,
                };
            }

            (var reqTribe, var prerequisites) = BuildingsData.GetPrerequisiteBuildings(building);
            if (reqTribe != TribeEnums.Any && reqTribe != tribe) return false;
            foreach (var prerequisite in prerequisites)
            {
                if (!IsBuildingAboveLevel(context, planManager, villageId, prerequisite.Building, prerequisite.Level)) return false;
            }
            return true;
        }

        public static bool IsBuildingAboveLevel(AppDbContext context, IPlanManager planManager, int villageId, BuildingEnums building, int lvl)
        {
            return (context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && lvl <= x.Level) ||
                    planManager.GetList(villageId).Any(x => x.Building == building && lvl <= x.Level));
        }

        public static bool IsResourceField(BuildingEnums building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }
    }
}