using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.Sim;
using TbsCore.TravianData;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Helpers
{
    public static class BuildingHelper
    {
        public static List<string> SetPrereqCombo(Account acc, Village vill)
        {
            var ret = new List<string>();
            for (int i = 5; i <= 45; i++) // Go through each building
            {
                var building = (BuildingEnum)i;

                // Don't show these buildings:
                if (building == BuildingEnum.GreatWarehouse ||
                    building == BuildingEnum.GreatGranary
                    ) continue;

                if (vill.Build.Buildings.Any(x => x.Type == building)) continue;
                if (vill.Build.Tasks.Any(x => x.Building == building)) continue;

                (var reqTribe, var prerequisites) = BuildingsData.GetBuildingPrerequisites((BuildingEnum)i);

                if (reqTribe != TribeEnum.Any && reqTribe != acc.AccInfo.Tribe) continue;

                ret.Add(building.ToString());
            }

            return ret;
        }

        public static void ReStartDemolishing(Account acc, Village vill)
        {
            if (vill.Build.DemolishTasks.Count <= 0) return;

            acc.Tasks.Add(new DemolishBuilding() { Vill = vill, ExecuteAt = DateTime.Now.AddSeconds(10) }, true, vill);
        }

        public static bool BuildingRequirementsAreMet(BuildingEnum building, Village vill, TribeEnum tribe) //check if user can construct this building
        {
            bool exists = (vill.Build.Buildings.FirstOrDefault(x => x.Type == building) != null); //there is already a building of this type in the vill
            if (exists)
            {
                //check cranny/warehouse/grannary/trapper/GG/GW
                switch (building)
                {
                    case BuildingEnum.Warehouse: return BuildingAboveLevel(BuildingEnum.Warehouse, 20, vill);
                    case BuildingEnum.Granary: return BuildingAboveLevel(BuildingEnum.Granary, 20, vill);
                    case BuildingEnum.GreatWarehouse: return BuildingAboveLevel(BuildingEnum.GreatWarehouse, 20, vill);
                    case BuildingEnum.GreatGranary: return BuildingAboveLevel(BuildingEnum.GreatGranary, 20, vill);
                    case BuildingEnum.Trapper: return BuildingAboveLevel(BuildingEnum.Trapper, 20, vill);
                    case BuildingEnum.Cranny: return BuildingAboveLevel(BuildingEnum.Cranny, 10, vill);
                    default: return false;
                }
            }

            //check for prerequisites for this building
            (var ReqTribe, var Prerequisites) = BuildingsData.GetBuildingPrerequisites(building);
            if (ReqTribe != TribeEnum.Any && ReqTribe != tribe) return false;
            //if we either already have this building OR is on our build task list, requirements are met.
            foreach (var prerequisite in Prerequisites)
            {
                if (!vill.Build.Buildings.Any(x => x.Level >= prerequisite.Level && x.Type == prerequisite.Building)
                    && !vill.Build.Tasks.Any(x => x.Level >= prerequisite.Level && x.Building == prerequisite.Building)) return false;
            }
            return true;
        }

        /// <summary>
        /// Whether there's a building above specific level or there's a task for this building
        /// </summary>
        public static bool BuildingAboveLevel(BuildingEnum building, int lvl, Village vill)
        {
            return (vill.Build.Buildings.Any(x => x.Type == building && lvl <= x.Level) ||
                    vill.Build.Tasks.Any(x => x.Building == building && lvl <= x.Level));
        }

        public static bool IsResourceField(BuildingEnum building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }

        public static bool IsWall(BuildingEnum building)
        {
            switch (building)
            {
                // Teutons
                case BuildingEnum.EarthWall:
                // Romans
                case BuildingEnum.CityWall:
                // Gauls
                case BuildingEnum.Palisade:
                // Egyptians
                case BuildingEnum.StoneWall:
                // Huns
                case BuildingEnum.MakeshiftWall:
                    return true;

                default:
                    return false;
            }
        }

        #region Functions for auto-building resource fields

        public static Building FindLowestLevelBuilding(List<Building> buildings)
        {
            return buildings
                .OrderBy(x => x.Level + (x.UnderConstruction ? 1 : 0))
                .FirstOrDefault();
        }

        public static Building GetLowestProduction(List<Building> buildings, Village vill)
        {
            // Get distinct field types
            var distinct = buildings.Select(x => x.Type).Distinct().ToList();

            var prodArr = vill.Res.Production.ToArray();
            var dict = new Dictionary<BuildingEnum, long>();
            for (int i = 0; i < 4; i++)
            {
                var resField = (BuildingEnum)i + 1;
                if (!distinct.Any(x => x == resField)) continue;
                dict.Add(resField, prodArr[i]);
            }

            var toUpgrade = dict.First(x => x.Value == dict.Min(y => y.Value)).Key;
            return FindLowestLevelBuilding(buildings.Where(x => x.Type == toUpgrade).ToList());
        }

        public static Building GetLowestRes(Account acc, Village vill, List<Building> buildings)
        {
            //get distinct field types
            var distinct = buildings.Select(x => x.Type).Distinct().ToList();
            long lowestRes = long.MaxValue;
            BuildingEnum toUpgrade = BuildingEnum.Cropland;

            var heroRes = vill.Settings.UseHeroRes ?
                HeroHelper.GetHeroResources(acc) :
                new long[] { 0, 0, 0, 0 };

            var resSum = ResourcesHelper.SumArr(vill.Res.Stored.Resources.ToArray(), heroRes);

            foreach (var distinctType in distinct)
            {
                if (distinctType == BuildingEnum.Woodcutter &&
                    resSum[0] < lowestRes)
                {
                    lowestRes = resSum[0];
                    toUpgrade = BuildingEnum.Woodcutter;
                }
                else if (distinctType == BuildingEnum.ClayPit &&
                    resSum[1] < lowestRes)
                {
                    lowestRes = resSum[1];
                    toUpgrade = BuildingEnum.ClayPit;
                }
                else if (distinctType == BuildingEnum.IronMine &&
                    resSum[2] < lowestRes)
                {
                    lowestRes = resSum[2];
                    toUpgrade = BuildingEnum.IronMine;
                }
                else if (distinctType == BuildingEnum.Cropland &&
                    resSum[3] < lowestRes)
                {
                    lowestRes = resSum[3];
                    toUpgrade = BuildingEnum.Cropland;
                }
            }
            return FindLowestLevelBuilding(buildings.Where(x => x.Type == toUpgrade).ToList());
        }

        private static bool CheckExcludeCrop(Village vill, BuildingTask task)
        {
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == BuildingEnum.Woodcutter))
            {
                if (res.Level < task.Level)
                {
                    if (res.Level == task.Level - 1 && res.UnderConstruction) continue; //if on level below + is being upgraded to that lvl
                    return false;
                }
            }
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == BuildingEnum.ClayPit))
            {
                if (res.Level < task.Level)
                {
                    if (res.Level == task.Level - 1 && res.UnderConstruction) continue; //if on level below + is being upgraded to that lvl
                    return false;
                }
            }
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == BuildingEnum.IronMine))
            {
                if (res.Level < task.Level)
                {
                    if (res.Level == task.Level - 1 && res.UnderConstruction) continue; //if on level below + is being upgraded to that lvl
                    return false;
                }
            }
            return true;
        }

        private static bool CheckOnlyCrop(Village vill, BuildingTask task)
        {
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == BuildingEnum.Cropland))
            {
                if (res.Level < task.Level)
                {
                    if (res.Level == task.Level - 1 && res.UnderConstruction) continue; //if on elevel below + is being upgraded to that lvl
                    return false;
                }
            }
            return true;
        }

        #endregion Functions for auto-building resource fields
    }
}