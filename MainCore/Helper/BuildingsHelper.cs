using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Services;
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

            (var reqTribe, var prerequisites) = building.GetPrerequisiteBuildings();
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

        public static (TribeEnums, List<PrerequisiteBuilding>) GetPrerequisiteBuildings(this BuildingEnums building)
        {
            TribeEnums tribe = TribeEnums.Any;
            var ret = new List<PrerequisiteBuilding>();
            switch (building)
            {
                case BuildingEnums.Sawmill:
                    ret.Add(new() { Building = BuildingEnums.Woodcutter, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Brickyard:
                    ret.Add(new() { Building = BuildingEnums.ClayPit, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.IronFoundry:
                    ret.Add(new() { Building = BuildingEnums.IronMine, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.GrainMill:
                    ret.Add(new() { Building = BuildingEnums.Cropland, Level = 5 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Bakery:
                    ret.Add(new() { Building = BuildingEnums.Cropland, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    ret.Add(new() { Building = BuildingEnums.GrainMill, Level = 5 });
                    break;

                case BuildingEnums.Warehouse:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Granary:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Blacksmith:
                    //DOESN'T EXIST ANYMORE
                    tribe = TribeEnums.Nature; //Just a dirty hack, since user can't be Nature, he can't build Blacksmith
                    break;

                case BuildingEnums.Smithy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 1 });
                    break;

                case BuildingEnums.TournamentSquare:
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 15 });
                    break;

                case BuildingEnums.MainBuilding:
                    break;

                case BuildingEnums.RallyPoint:
                    break;

                case BuildingEnums.Marketplace:
                    ret.Add(new() { Building = BuildingEnums.Warehouse, Level = 1 });
                    ret.Add(new() { Building = BuildingEnums.Granary, Level = 1 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    break;

                case BuildingEnums.Embassy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Barracks:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.Stable:
                    ret.Add(new() { Building = BuildingEnums.Smithy, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 5 });
                    break;

                case BuildingEnums.Workshop:
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Academy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Barracks, Level = 3 });
                    break;

                case BuildingEnums.Cranny:
                    break;

                case BuildingEnums.TownHall:
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 });
                    break;

                case BuildingEnums.Residence:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //no palace!
                    break;

                case BuildingEnums.Palace:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //no residence!
                    ret.Add(new() { Building = BuildingEnums.Embassy, Level = 1 });
                    break;

                case BuildingEnums.Treasury:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 });
                    break;

                case BuildingEnums.TradeOffice:
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.Marketplace, Level = 20 });
                    break;

                case BuildingEnums.GreatBarracks:
                    ret.Add(new() { Building = BuildingEnums.Barracks, Level = 20 }); //not capital!
                    break;

                case BuildingEnums.GreatStable:
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 20 }); //not capital
                    break;

                case BuildingEnums.CityWall:
                    tribe = TribeEnums.Romans;
                    break;

                case BuildingEnums.EarthWall:
                    tribe = TribeEnums.Teutons;
                    break;

                case BuildingEnums.Palisade:
                    tribe = TribeEnums.Gauls;
                    break;

                case BuildingEnums.StonemasonsLodge:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //capital
                    break;

                case BuildingEnums.Brewery:
                    tribe = TribeEnums.Teutons;
                    ret.Add(new() { Building = BuildingEnums.Granary, Level = 20 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    break;

                case BuildingEnums.Trapper:
                    tribe = TribeEnums.Gauls;
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.HerosMansion:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.GreatWarehouse:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnums.GreatGranary:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnums.WW: //ww vill
                    tribe = TribeEnums.Nature; //Just a dirty hack, since user can't be Nature, bot can't construct WW.
                    break;

                case BuildingEnums.HorseDrinkingTrough:
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 20 });
                    tribe = TribeEnums.Romans;
                    break;

                case BuildingEnums.StoneWall:
                    tribe = TribeEnums.Egyptians;
                    break;

                case BuildingEnums.MakeshiftWall:
                    tribe = TribeEnums.Huns;
                    break;

                case BuildingEnums.CommandCenter: //no res/palace
                    tribe = TribeEnums.Huns;
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Waterworks:
                    tribe = TribeEnums.Egyptians;
                    ret.Add(new() { Building = BuildingEnums.HerosMansion, Level = 10 });
                    break;

                default:
                    break;
            }
            return (tribe, ret);
        }
    }
}