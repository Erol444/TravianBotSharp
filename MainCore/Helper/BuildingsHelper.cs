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
            var result = new List<BuildingEnums>();
            var tribe = context.AccountsInfo.Find(accountId).Tribe;
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
            if (IsExists(context, planManager, villageId, building))
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
                if (prerequisite.Building.IsResourceField())
                {
                    if (prerequisite.Building == BuildingEnums.Cropland)
                    {
                        if (IsAutoCropFieldAboveLevel(planManager, villageId, prerequisite.Level)) return true;
                    }
                    else
                    {
                        if (IsAutoResourceFieldAboveLevel(planManager, villageId, prerequisite.Level)) return true;
                    }
                }
                if (!IsBuildingAboveLevel(context, planManager, villageId, prerequisite.Building, prerequisite.Level)) return false;
            }
            return true;
        }

        private static bool IsExists(AppDbContext context, IPlanManager planManager, int villageId, BuildingEnums building)
        {
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == building);
            if (b is not null) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == building);
            if (c is not null) return true;
            var q = planManager.GetList(villageId).FirstOrDefault(x => x.Building == building);
            if (q is not null) return true;
            return false;
        }

        private static bool IsBuildingAboveLevel(AppDbContext context, IPlanManager planManager, int villageId, BuildingEnums building, int lvl)
        {
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && lvl <= x.Level);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && lvl <= x.Level);
            if (c) return true;
            var q = planManager.GetList(villageId).Any(x => x.Building == building && lvl <= x.Level);
            if (q) return true;
            return false;
        }

        private static bool IsAutoResourceFieldAboveLevel(IPlanManager planManager, int villageId, int lvl)
        {
            return planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.ExcludeCrop) && lvl <= x.Level);
        }

        private static bool IsAutoCropFieldAboveLevel(IPlanManager planManager, int villageId, int lvl)
        {
            return planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.OnlyCrop) && lvl <= x.Level);
        }

        public static bool IsResourceField(this BuildingEnums building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }

        public static bool IsNotAdsUpgrade(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.CommandCenter => true,
                BuildingEnums.Palace => true,
                BuildingEnums.Residence => true,
                _ => false,
            };
        }

        public static bool IsWall(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.EarthWall => true,
                BuildingEnums.CityWall => true,
                BuildingEnums.Palisade => true,
                BuildingEnums.StoneWall => true,
                BuildingEnums.MakeshiftWall => true,
                _ => false,
            };
        }

        public static BuildingEnums GetWall(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Teutons => BuildingEnums.EarthWall,
                TribeEnums.Romans => BuildingEnums.CityWall,
                TribeEnums.Gauls => BuildingEnums.Palisade,
                TribeEnums.Egyptians => BuildingEnums.StoneWall,
                TribeEnums.Huns => BuildingEnums.MakeshiftWall,
                _ => BuildingEnums.Site,
            };
        }

        public static bool IsMultipleAllow(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Warehouse => true,
                BuildingEnums.Granary => true,
                BuildingEnums.GreatWarehouse => true,
                BuildingEnums.GreatGranary => true,
                BuildingEnums.Trapper => true,
                BuildingEnums.Cranny => true,
                _ => false,
            };
        }

        public static int GetMaxLevel(this BuildingEnums building)
        {
            return building switch
            {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                BuildingEnums.Brewery => 20,
#elif TTWARS
                BuildingEnums.Brewery => 10,
                BuildingEnums.Woodcutter => 25,
                BuildingEnums.ClayPit => 25,
                BuildingEnums.IronMine => 25,
                BuildingEnums.Cropland => 25,
#else

#error You forgot to define Travian version here

#endif
                BuildingEnums.Bakery => 5,
                BuildingEnums.Brickyard => 5,
                BuildingEnums.IronFoundry => 5,
                BuildingEnums.GrainMill => 5,
                BuildingEnums.Sawmill => 5,

                BuildingEnums.Cranny => 10,
                _ => 20,
            };
        }

        public static string GetColor(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Site => "White",
                BuildingEnums.Woodcutter => "ForestGreen",
                BuildingEnums.ClayPit => "Orange",
                BuildingEnums.IronMine => "Gray",
                BuildingEnums.Cropland => "Yellow",
                _ => "LawnGreen",
            };
        }

        public static BuildingEnums GetTribesWall(this TribeEnums tribe) => tribe switch
        {
            TribeEnums.Teutons => BuildingEnums.EarthWall,
            TribeEnums.Romans => BuildingEnums.CityWall,
            TribeEnums.Gauls => BuildingEnums.Palisade,
            TribeEnums.Egyptians => BuildingEnums.StoneWall,
            TribeEnums.Huns => BuildingEnums.MakeshiftWall,
            _ => BuildingEnums.Site,
        };

        public static bool HasMultipleTabs(this BuildingEnums building) => building switch
        {
            BuildingEnums.RallyPoint => true,
            BuildingEnums.CommandCenter => true,
            BuildingEnums.Residence => true,
            BuildingEnums.Palace => true,
            BuildingEnums.Marketplace => true,
            BuildingEnums.Treasury => true,
            _ => false,
        };

        public static int GetBuildingsCategory(this BuildingEnums building) => building switch
        {
            BuildingEnums.GrainMill => 2,
            BuildingEnums.Sawmill => 2,
            BuildingEnums.Brickyard => 2,
            BuildingEnums.IronFoundry => 2,
            BuildingEnums.Bakery => 2,
            BuildingEnums.Barracks => 1,
            BuildingEnums.HerosMansion => 1,
            BuildingEnums.Academy => 1,
            BuildingEnums.Smithy => 1,
            BuildingEnums.Stable => 1,
            BuildingEnums.GreatBarracks => 1,
            BuildingEnums.GreatStable => 1,
            BuildingEnums.Workshop => 1,
            BuildingEnums.TournamentSquare => 1,
            BuildingEnums.Trapper => 1,
            _ => 0,
        };
    }
}