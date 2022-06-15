using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;

using static TbsCore.Helpers.Classificator;

namespace TbsCore.TravianData
{
    public static class BuildingsData
    {
        /// <summary>
        /// Get the building cost for specific level
        /// </summary>
        /// <param name="building"></param>
        /// <param name="level"></param>
        public static long[] GetBuildingCost(Classificator.BuildingEnum building, int level)
        {
            long[] ret = new long[4];
            var baseCost = BuildingCost.GetRow((int)building);

            for (int i = 0; i < 4; i++)
            {
                var cost = baseCost[i] * Math.Pow(baseCost[4], level - 1);
                ret[i] = (long)Math.Round(cost / 5.0) * 5;
            }
            return ret;
        }

        /// <summary>
        /// Whether there can be multiple buildings of this type in a village
        /// </summary>
        /// <param name="building">Building</param>
        /// <returns></returns>
        public static bool CanHaveMultipleBuildings(BuildingEnum building) =>
            multipleBuildingsAllowes.Any(x => x == building);

        private static readonly float[,] WallData = new float[,] {
            { 1.030F, 10}, // City wall
            { 1.020F, 6}, // Earth wall
            { 1.025F, 8}, // Palisade
            { 1.0F, 0}, // {Nature}
            { 1.0F, 0}, // {Natars}
            { 1.025F, 8}, // Stone wall
            { 1.015F, 6}, // Makeshift wall
        };

        /// <summary>
        /// Gets wall deffensive bonus and basic deffensive power. Used by the combat simulator.
        /// </summary>
        public static (double, int) GetWallBonus(TribeEnum tribe, int wallLevel)
        {
            var tribeIndex = (int)tribe - 1;

            var bonus = Math.Pow(WallData[tribeIndex, 0], wallLevel);
            var basicDeff = (int)WallData[tribeIndex, 1] * wallLevel;

            return (bonus, basicDeff);
        }

        /// <summary>
        /// Gets the highest level a building can be on
        /// </summary>
        /// <param name="building">Building</param>
        /// <returns>Max level</returns>
        public static int MaxBuildingLevel(Account acc, BuildingEnum building)
        {
            switch (building)
            {
                case BuildingEnum.Brewery:
                    return 20;

                case BuildingEnum.Bakery:
                case BuildingEnum.Brickyard:
                case BuildingEnum.IronFoundry:
                case BuildingEnum.GrainMill:
                case BuildingEnum.Sawmill:
                    return 5;

                case BuildingEnum.Cranny: return 10;

                default: return 20;
            }
        }

        public static BuildingEnum GetTribesWall(Classificator.TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Teutons:
                    return BuildingEnum.EarthWall;

                case TribeEnum.Romans:
                    return BuildingEnum.CityWall;

                case TribeEnum.Gauls:
                    return BuildingEnum.Palisade;

                case TribeEnum.Egyptians:
                    return BuildingEnum.StoneWall;

                case TribeEnum.Huns:
                    return BuildingEnum.MakeshiftWall;

                default:
                    return BuildingEnum.Site;
            }
        }

        private static readonly float[,] BuildingCost = new float[,] {
            {0,0,0,0,0F}, //Site
            {  40, 100,  50,  60, 1.67F}, //Woodcutter
            {  80,  40,  80,  50, 1.67F}, //Claypit
            { 100,  80,  30,  60, 1.67F},
            {  70,  90,  70,  20, 1.67F},
            { 520, 380, 290,  90, 1.80F},
            { 440, 480, 320,  50, 1.80F},
            { 200, 450, 510, 120, 1.80F},
            { 500, 440, 380,1240, 1.80F},
            {1200,1480, 870,1600, 1.80F},
            { 130, 160,  90,  40, 1.28F},
            {  80, 100,  70,  20, 1.28F},
            { 130, 210, 410, 130, 1.28F},
            {180, 250, 500, 160, 1.28F},
            {1750,2250,1530, 240, 1.28F},
            {  70,  40,  60,  20, 1.28F},
            { 110, 160,  90,  70, 1.28F},
            {  80,  70, 120,  70, 1.28F},
            { 180, 130, 150,  80, 1.28F},
            { 210, 140, 260, 120, 1.28F},
            { 260, 140, 220, 100, 1.28F},
            { 460, 510, 600, 320, 1.28F},
            { 220, 160,  90,  40, 1.28F},
            {  40,  50,  30,  10, 1.28F},
            {1250,1110,1260, 600, 1.28F},
            { 580, 460, 350, 180, 1.28F},
            { 550, 800, 750, 250, 1.28F},
            {2880,2740,2580, 990, 1.26F},
            {1400,1330,1200, 400, 1.28F},
            { 630, 420, 780, 360, 1.28F},
            { 780, 420, 660, 300, 1.28F},
            {  70,  90, 170,  70,1.28F},
            { 120, 200,   0,  80,1.28F},
            { 160, 100,  80,  60,1.28F},
            { 155, 130, 125,  70, 1.28F},
            {1460, 930,1250,1740, 1.40F},
            { 100, 100, 100, 100, 1.28F},
            { 700, 670, 700, 240, 1.33F},
            {650,800, 450, 200, 1.28F},
            { 400, 500, 350, 100, 1.28F},
            {66700,69050,72200,13200,1.0275F},
            { 780, 420, 660, 540, 1.28F},
            { 110, 160,  70,  60, 1.28F},
            {  50,  80,  40,  30, 1.28F},
            {1600,1250,1050, 200, 1.22F},
            { 910, 945, 910, 340, 1.31F}
        };

        private static readonly BuildingEnum[] multipleBuildingsAllowes = new BuildingEnum[] {
            BuildingEnum.Warehouse,
            BuildingEnum.Granary,
            BuildingEnum.GreatWarehouse,
            BuildingEnum.GreatGranary,
            BuildingEnum.Trapper,
            BuildingEnum.Cranny,
            // Resource fields
            BuildingEnum.Woodcutter,
            BuildingEnum.ClayPit,
            BuildingEnum.IronMine,
            BuildingEnum.Cropland,
        };

        public static BuildingCategoryEnum GetBuildingsCategory(BuildingEnum building)
        {
            switch (building)
            {
                case BuildingEnum.GrainMill:
                case BuildingEnum.Sawmill:
                case BuildingEnum.Brickyard:
                case BuildingEnum.IronFoundry:
                case BuildingEnum.Bakery:
                    return BuildingCategoryEnum.Resources;

                case BuildingEnum.RallyPoint:
                case BuildingEnum.EarthWall:
                case BuildingEnum.CityWall:
                case BuildingEnum.MakeshiftWall:
                case BuildingEnum.StoneWall:
                case BuildingEnum.Palisade:
                case BuildingEnum.Barracks:
                case BuildingEnum.HerosMansion:
                case BuildingEnum.Academy:
                case BuildingEnum.Smithy:
                case BuildingEnum.Stable:
                case BuildingEnum.GreatBarracks:
                case BuildingEnum.GreatStable:
                case BuildingEnum.Workshop:
                case BuildingEnum.TournamentSquare:
                case BuildingEnum.Trapper:
                    return BuildingCategoryEnum.Military;

                default:
                    return BuildingCategoryEnum.Infrastructure;
            }
        }

        public static (TribeEnum, List<Prerequisite>) GetBuildingPrerequisites(BuildingEnum building)
        {
            TribeEnum tribe = TribeEnum.Any;
            var ret = new List<Prerequisite>();
            switch (building)
            {
                case BuildingEnum.Woodcutter:
                case BuildingEnum.ClayPit:
                case BuildingEnum.IronMine:
                case BuildingEnum.Cropland:
                    break;

                case BuildingEnum.Sawmill:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Woodcutter, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    break;

                case BuildingEnum.Brickyard:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.ClayPit, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    break;

                case BuildingEnum.IronFoundry:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.IronMine, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    break;

                case BuildingEnum.GrainMill:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Cropland, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    break;

                case BuildingEnum.Bakery:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Cropland, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.GrainMill, Level = 5 });
                    break;

                case BuildingEnum.Warehouse:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 1 });
                    break;

                case BuildingEnum.Granary:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 1 });
                    break;

                case BuildingEnum.Blacksmith:
                    //DOESN'T EXIST ANYMORE
                    tribe = TribeEnum.Nature; //Just a dirty hack, since user can't be Nature, he can't build Blacksmith
                    break;

                case BuildingEnum.Smithy:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    break;

                case BuildingEnum.TournamentSquare:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 15 });
                    break;

                case BuildingEnum.MainBuilding:
                    break;

                case BuildingEnum.RallyPoint:
                    break;

                case BuildingEnum.Marketplace:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Warehouse, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Granary, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 3 });
                    break;

                case BuildingEnum.Embassy:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 1 });
                    break;

                case BuildingEnum.Barracks:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 1 });
                    break;

                case BuildingEnum.Stable:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    break;

                case BuildingEnum.Workshop:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    break;

                case BuildingEnum.Academy:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Barracks, Level = 3 });
                    break;

                case BuildingEnum.Cranny:
                    break;

                case BuildingEnum.TownHall:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 10 });
                    break;

                case BuildingEnum.Residence:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 }); //no palace!
                    break;

                case BuildingEnum.Palace:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 }); //no residence!
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Embassy, Level = 1 });
                    break;

                case BuildingEnum.Treasury:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 10 });
                    break;

                case BuildingEnum.TradeOffice:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Marketplace, Level = 20 });
                    break;

                case BuildingEnum.GreatBarracks:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Barracks, Level = 20 }); //not capital!
                    break;

                case BuildingEnum.GreatStable:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 20 }); //not capital
                    break;

                case BuildingEnum.CityWall:
                    tribe = TribeEnum.Romans;
                    break;

                case BuildingEnum.EarthWall:
                    tribe = TribeEnum.Teutons;
                    break;

                case BuildingEnum.Palisade:
                    tribe = TribeEnum.Gauls;
                    break;

                case BuildingEnum.StonemasonsLodge:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 }); //capital
                    break;

                case BuildingEnum.Brewery:
                    tribe = TribeEnum.Teutons;
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Granary, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    break;

                case BuildingEnum.Trapper:
                    tribe = TribeEnum.Gauls;
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 1 });
                    break;

                case BuildingEnum.HerosMansion:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 1 });
                    break;

                case BuildingEnum.GreatWarehouse:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnum.GreatGranary:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnum.WW: //ww vill
                    tribe = TribeEnum.Nature; //Just a dirty hack, since user can't be Nature, bot can't construct WW.
                    break;

                case BuildingEnum.HorseDrinkingTrough:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 20 });
                    tribe = TribeEnum.Romans;
                    break;

                case BuildingEnum.StoneWall:
                    tribe = TribeEnum.Egyptians;
                    break;

                case BuildingEnum.MakeshiftWall:
                    tribe = TribeEnum.Huns;
                    break;

                case BuildingEnum.CommandCenter: //no res/palace
                    tribe = TribeEnum.Huns;
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    break;

                case BuildingEnum.Waterworks:
                    tribe = TribeEnum.Egyptians;
                    ret.Add(new Prerequisite() { Building = BuildingEnum.HerosMansion, Level = 10 });
                    break;

                default: break;
            }
            return (tribe, ret);
        }

        /// <summary>
        /// Whether building has multiple tabs inside
        /// </summary>
        public static bool HasMultipleTabs(BuildingEnum building) =>
            multipleTabsBuildings.Any(x => x == building);

        /// <summary>
        /// Buildings with multiple tabs inside
        /// </summary>
        private static BuildingEnum[] multipleTabsBuildings = new BuildingEnum[] {
            BuildingEnum.RallyPoint,
            BuildingEnum.CommandCenter,
            BuildingEnum.Residence,
            BuildingEnum.Palace,
            BuildingEnum.Marketplace,
            BuildingEnum.Treasury,
        };

        /// <summary>
        /// Buildings that are always build in the same spot
        /// </summary>
        //public static BuildingEnum[] StaticBuildings = new BuildingEnum[] {
        //    BuildingEnum.Wall,
        //    BuildingEnum.MakeshiftWall,
        //    BuildingEnum.Palisade,
        //    BuildingEnum.StoneWall,
        //    BuildingEnum.EarthWall,
        //    BuildingEnum.CityWall,
        //    BuildingEnum.RallyPoint,
        //    BuildingEnum.WW
        //};
    }
}