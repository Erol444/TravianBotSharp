using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.Models.BuildingModels;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Files.TravianData;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Helpers
{
    public static class BuildingHelper
    {
        /// <summary>
        /// Adds the building task to the village list of building tasks. Restarts BotTask UpgradeBuilding if needed.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="task">BuildingTask to add</param>
        /// <param name="bottom">Whether to insert the BuildingTask on the bottom of the list</param>
        /// <returns>Whether the method completed successfully</returns>
        public static bool AddBuildingTask(Account acc, Village vill, BuildingTask task, bool bottom = true)
        {
            if (task.BuildingId == null || 
                vill.Build.Buildings.Any(x=>x.Id == task.BuildingId && x.Type != task.Building && x.Type != BuildingEnum.Site))
            {
                //Check if bot has any space to build new buildings, otherwise return
                if (!FindBuildingId(vill, task)) return false;
            }
            if (bottom) vill.Build.Tasks.Add(task);
            else vill.Build.Tasks.Insert(0, task);

            if (acc.Wb != null) ReStartBuilding(acc, vill);
            return true;
        }

        /// <summary>
        /// Finds appropriate building space/site for the building
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="task">Building task</param>
        /// <returns>True if successful</returns>
        public static bool FindBuildingId(Village vill, BuildingTask task)
        {
            //auto field upgrade/demolish task, no need for Id
            if (task.TaskType == BuildingType.AutoUpgradeResFields) return true;

            var existingBuilding = vill.Build.Buildings
                    .FirstOrDefault(x => x.Type == task.Building);

            // Only special buildings (warehouse, cranny, granary etc.) can have multiple 
            // buildings of it's type and use ConstructNew option
            if (!BuildingsData.CanHaveMultipleBuildings(task.Building)) task.ConstructNew = false;
            
            if (existingBuilding != null && !task.ConstructNew)
            {
                task.BuildingId = existingBuilding.Id;
                return true;
            }

            var ExistingBuildingTask = vill.Build.Tasks
                    .FirstOrDefault(x => x.Building == task.Building && x.BuildingId != null);

            if (ExistingBuildingTask != null && !task.ConstructNew)
            {
                task.BuildingId = ExistingBuildingTask.BuildingId;
                return true;
            }

            var FreeSites = vill.Build.Buildings
                .Where(x => x.Type == BuildingEnum.Site && 19 <= x.Id && x.Id <= 39)
                .ToList();
            foreach (var FreeSite in FreeSites)
            {
                if (!vill.Build.Tasks.Any(x => x.BuildingId == FreeSite.Id))
                {
                    // Site is free and there's no building task that reserves it.
                    task.BuildingId = FreeSite.Id;
                    return true;
                }
            }
            return false;
        }

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


        public static void ReStartBuilding(Account acc, Village vill)
        {
            RemoveCompletedTasks(vill, acc);
            //remove ongoing building task for this village
            acc.Tasks.RemoveAll(x =>
                x.Vill == vill &&
                x.GetType() == typeof(UpgradeBuilding)
                );

            if (vill.Build.Tasks.Count == 0) return; //No build tasks

            var (_, nextExecution) = UpgradeBuildingHelper.NextBuildingTask(acc, vill);
            
            TaskExecutor.AddTask(acc, new UpgradeBuilding()
            {
                Vill = vill,
                ExecuteAt = nextExecution,
            });
        }

       
        public static void ReStartDemolishing(Account acc, Village vill)
        {
            if (vill.Build.DemolishTasks.Count <= 0) return;
            
            TaskExecutor.AddTaskIfNotExistInVillage(acc, 
                vill, 
                new DemolishBuilding() { Vill = vill, ExecuteAt = DateTime.Now.AddSeconds(10) }
                );
        }

        public static bool BuildingRequirementsAreMet(BuildingEnum building, Village vill, TribeEnum tribe) //check if user can construct this building
        {
            bool exists = (vill.Build.Buildings.FirstOrDefault(x => x.Type == building) != null); //there is already a building of this type in the vill
            if (exists)
            {
                //check cranny/warehouse/grannary/trapper/GG/GW
                switch (building)
                {
                    case BuildingEnum.Warehouse: return BuildingIsOnLevel(BuildingEnum.Warehouse, 20, vill);
                    case BuildingEnum.Granary: return BuildingIsOnLevel(BuildingEnum.Granary, 20, vill);
                    case BuildingEnum.GreatWarehouse: return BuildingIsOnLevel(BuildingEnum.GreatWarehouse, 20, vill);
                    case BuildingEnum.GreatGranary: return BuildingIsOnLevel(BuildingEnum.GreatGranary, 20, vill);
                    case BuildingEnum.Trapper: return BuildingIsOnLevel(BuildingEnum.Trapper, 20, vill);
                    case BuildingEnum.Cranny: return BuildingIsOnLevel(BuildingEnum.Cranny, 10, vill);
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

        private static bool BuildingIsOnLevel(BuildingEnum building, int lvl, Village vill)
        {
            //if there already is a building on specific level or there is a task for this building
            // TODO: change FristOrDefault to Any
            return (vill.Build.Buildings.FirstOrDefault(x => x.Level == lvl && x.Type == building) != null || vill.Build.Tasks.FirstOrDefault(x => x.Level == lvl && x.Building == building) != null);
        }

        public static void RemoveFinishedCB(Village vill)
        {
            vill.Build.CurrentlyBuilding.RemoveAll(x => x.Duration < DateTime.Now);
        }

        /// <summary>
        /// Used by building task to get the url for navigation
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="task">BotTask</param>
        /// <returns></returns>
        public static (string, bool) GetUrlForBuilding(Village vill, BuildingTask task)
        {
            switch (task.TaskType)
            {
                case BuildingType.General:
                    return GetUrlGeneralTask(vill, task);
                case BuildingType.AutoUpgradeResFields:
                    return (GetUrlAutoResFields(vill, task), false);
            }
            return (null, true);
        }

        public static bool IsTaskCompleted(Village vill, Account acc, BuildingTask task)
        {
            if (vill == null) return true;
            var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId);
            switch (task.TaskType)
            {
                case BuildingType.General:

                    if (building == null)
                    {
                        if (vill.Build.Buildings.Any(x => x.Type == task.Building) &&
                            vill.Build.Buildings.FirstOrDefault(x => x.Type == task.Building).Level >= task.Level)
                        {
                            //if ((this.Building == Type.Residence) && (this.Level == 10 || this.Level == 20)) TrainSettlers(vill, acc, 0);
                            return true;
                        }
                        return false; //this building doest exist yet!
                    }
                    if (building.Level >= task.Level || (building.Level + 1 == task.Level && building.UnderConstruction))
                    {
                        //if (this.Building == Type.Residence) TrainSettlers(vill, acc, 30);
                        return true;
                    }
                    return false;

                case BuildingType.AutoUpgradeResFields:
                    if (vill.Build.Buildings[0].Type == BuildingEnum.Site) return false; //for new villages that are not checked yet
                    switch (task.ResourceType)
                    {
                        case ResTypeEnum.AllResources:
                            return (CheckOnlyCrop(vill, task) && CheckExcludeCrop(vill, task));
                        case ResTypeEnum.ExcludeCrop:
                            return CheckExcludeCrop(vill, task);
                        case ResTypeEnum.OnlyCrop:
                            return CheckOnlyCrop(vill, task);
                        default:
                            return true;
                    }
            }
            return false;
        }
        private static (string, bool) GetUrlGeneralTask(Village vill, BuildingTask task)
        {
            // Check if there is already a different building in this spot
            if (task.BuildingId == null || vill.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId).Type != task.Building)
            {
                var targetBuilding = vill.Build.Buildings.FirstOrDefault(x => x.Type == task.Building);
                // Re-select the buildingId
                if (targetBuilding != null && !task.ConstructNew)
                {
                    task.BuildingId = targetBuilding.Id;
                }
                else // there's already a building in this spot, construct a building elsewhere
                {
                    if (!BuildingHelper.FindBuildingId(vill, task))
                    {
                        return (null, false);
                    }
                }
            }

            var url = task.BuildingId.ToString();

            bool constructNew = false;
            // If there is no building in that space currently, construct a new building
            if (vill.Build.Buildings.Any(x => x.Type == BuildingEnum.Site && x.Id == task.BuildingId))
            {
                url += "&category=" + (int)BuildingsData.GetBuildingsCategory(task.Building);
                constructNew = true;
            }
            return (url, constructNew);
        }
        public static string GetUrlAutoResFields(Village vill, BuildingTask task)
        {
            List<Models.ResourceModels.Building> buildings; // Potential buildings to be upgraded next
            switch (task.ResourceType)
            {
                case ResTypeEnum.AllResources:
                    buildings = vill.Build.Buildings.Where(x => x.Type == BuildingEnum.Woodcutter || x.Type == BuildingEnum.ClayPit || x.Type == BuildingEnum.IronMine || x.Type == BuildingEnum.Cropland).ToList();
                    break;
                case ResTypeEnum.ExcludeCrop:
                    buildings = vill.Build.Buildings.Where(x => x.Type == BuildingEnum.Woodcutter || x.Type == BuildingEnum.ClayPit || x.Type == BuildingEnum.IronMine).ToList();
                    break;
                case ResTypeEnum.OnlyCrop:
                    buildings = vill.Build.Buildings.Where(x => x.Type == BuildingEnum.Cropland).ToList();
                    break;
                default:
                    return null;
            }
            buildings = buildings.Where(x => x.Level < task.Level).ToList(); // Only select res fields that are below desired level
            foreach (var b in buildings.ToList())
            {
                if (b.Level == task.Level - 1 && b.UnderConstruction) buildings.Remove(b); // It's already being upgraded to selected level
            }

            // Filter resource fields by type
            //buildings = buildings.Where(x => x.Type == task.Building).ToList();

            Models.ResourceModels.Building buildingToUpgrade = null;
            switch (task.BuildingStrategy)
            {
                case BuildingStrategyEnum.BasedOnLevel:
                    buildingToUpgrade = FindLowestLevelBuilding(buildings);
                    break;
                case BuildingStrategyEnum.BasedOnProduction:
                    buildingToUpgrade = GetLowestProduction(buildings, vill);
                    break;
                case BuildingStrategyEnum.BasedOnRes:
                    buildingToUpgrade = GetLowestRes(buildings, vill);
                    break;
            }
            if (buildingToUpgrade == null)
            {
                vill.Build.Tasks.Remove(task);
                return null;
            }
            task.BuildingId = buildingToUpgrade.Id;
            task.Building = buildingToUpgrade.Type;

            return buildingToUpgrade.Id.ToString();
        }

        /// <summary>
        /// Upgrades specified building for exactly one level. Will upgrade the lowest level building.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="building">Building to be upgraded by one</param>
        /// <param name="bottom">Whether to insert the building task on the bottom of the build list</param>
        /// <returns>Whether the method executed successfully</returns>
        internal static bool UpgradeBuildingForOneLvl(Account acc, Village vill, BuildingEnum building, bool bottom = true)
        {
            // We already have a build task
            if (!bottom && vill.Build.Tasks.FirstOrDefault()?.Building == building) return true;
            if (bottom && vill.Build.Tasks.LastOrDefault()?.Building == building) return true;

            var upgrade = vill.Build
                .Buildings
                .OrderBy(x => x.Level)
                .FirstOrDefault(x => x.Type == building);

            // We don't have this building in the village yet
            if (upgrade == null)
            {
                return AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = building,
                    Level = 1,
                }, bottom);
            }

            var currentLvl = (int)upgrade.Level;

            RemoveFinishedCB(vill);
            currentLvl += vill.Build.CurrentlyBuilding.Count(x => x.Building == building);

            if (BuildingsData.MaxBuildingLevel(acc, building) == currentLvl)
            {
                // Building is on max level, construct new building if possible
                if (!BuildingsData.CanHaveMultipleBuildings(building)) return false;

                return AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = building,
                    Level = 1,
                }, bottom);
            }
            else // Upgrade the defined building
            {
                return AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = building,
                    Level = currentLvl + 1, 
                    BuildingId = upgrade.Id
                }, bottom);
            }
        }



        /// <summary>
        /// Adds all building prerequisites for this building if they do not exist yet. 
        /// After this you should call RemoveDuplicates().
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="vill"></param>
        /// <param name="building"></param>
        /// <returns>Whether we have all prerequisite buildings</returns>
        public static bool AddBuildingPrerequisites(Account acc, Village vill, BuildingEnum building, bool bottom = true)
        {
            (var tribe, var prereqs) = BuildingsData.GetBuildingPrerequisites(building);
            if (acc.AccInfo.Tribe != tribe && tribe != TribeEnum.Any) return false;
            if (prereqs.Count == 0) return true;
            var ret = true;
            foreach (var prereq in prereqs)
            {
                var prereqBuilding = vill.Build.Buildings.Where(x => x.Type == prereq.Building);
                
                // Prerequired building already exists and is on on/above desired level
                if (prereqBuilding.Any(x => prereq.Level <= x.Level)) continue;

                if (bottom && vill.Build.Tasks.Any(x => prereq.Building == x.Building &&
                                              prereq.Level <= x.Level)) continue;
                
                // If there is no required building, build it's prerequisites first
                if (!prereqBuilding.Any()) AddBuildingPrerequisites(acc, vill, prereq.Building);

                AddBuildingTask(acc, vill, new BuildingTask()
                {
                    Building = prereq.Building,
                    Level = prereq.Level,
                    TaskType = BuildingType.General
                }, bottom);
                ret = false;
            }
            return ret;
        }

        public static bool IsResourceField(BuildingEnum building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }

        /// <summary>
        /// Removes all complete building tasks
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="acc">Account</param>
        public static void RemoveCompletedTasks(Village vill, Account acc) =>
            vill.Build.Tasks.RemoveAll(task => IsTaskCompleted(vill, acc, task));

        #region Functions for auto-building resource fields
        public static Models.ResourceModels.Building FindLowestLevelBuilding(List<Models.ResourceModels.Building> buildings)
        {
            // TODO: test after implementation
            //return buildings
            //        .OrderBy(x => x.Level + (x.UnderConstruction ? 1 : 0))
            //        .FirstOrDefault();

            if (buildings.Count == 0) return null;
            int lowestLvl = 100;
            Models.ResourceModels.Building lowestBuilding = new Models.ResourceModels.Building();
            for (int i = 0; i < buildings.Count; i++)
            {
                var buildingLevel = buildings[i].Level;
                if (buildings[i].UnderConstruction) buildingLevel++;
                if (lowestLvl > buildingLevel)
                {
                    lowestLvl = buildingLevel;
                    lowestBuilding = buildings[i];
                }
            }
            return lowestBuilding;
        }
        private static Models.ResourceModels.Building GetLowestProduction(List<Models.ResourceModels.Building> buildings, Village vill)
        {
            //get distinct field types
            var distinct = buildings.Select(x => x.Type).Distinct().ToList();
            long lowestProd = long.MaxValue;
            BuildingEnum toUpgrade = BuildingEnum.Cropland;

            foreach (var distinctType in distinct)
            {
                if (distinctType == BuildingEnum.Woodcutter && vill.Res.Production.Wood < lowestProd) { lowestProd = vill.Res.Production.Wood; toUpgrade = BuildingEnum.Woodcutter; }
                if (distinctType == BuildingEnum.ClayPit && vill.Res.Production.Clay < lowestProd) { lowestProd = vill.Res.Production.Clay; toUpgrade = BuildingEnum.ClayPit; }
                if (distinctType == BuildingEnum.IronMine && vill.Res.Production.Iron < lowestProd) { lowestProd = vill.Res.Production.Iron; toUpgrade = BuildingEnum.IronMine; }
                if (distinctType == BuildingEnum.Cropland && vill.Res.Production.Crop < lowestProd) { lowestProd = vill.Res.Production.Crop; toUpgrade = BuildingEnum.Cropland; }
            }
            return FindLowestLevelBuilding(buildings.Where(x => x.Type == toUpgrade).ToList());
        }
        private static Models.ResourceModels.Building GetLowestRes(List<Models.ResourceModels.Building> buildings, Village vill)
        {
            //get distinct field types
            var distinct = buildings.Select(x => x.Type).Distinct().ToList();
            long lowestRes = long.MaxValue;
            BuildingEnum toUpgrade = BuildingEnum.Cropland;

            foreach (var distinctType in distinct)
            {
                if (distinctType == BuildingEnum.Woodcutter && vill.Res.Stored.Resources.Wood < lowestRes) { lowestRes = vill.Res.Stored.Resources.Wood; toUpgrade = BuildingEnum.Woodcutter; }
                if (distinctType == BuildingEnum.ClayPit && vill.Res.Stored.Resources.Clay < lowestRes) { lowestRes = vill.Res.Stored.Resources.Clay; toUpgrade = BuildingEnum.ClayPit; }
                if (distinctType == BuildingEnum.IronMine && vill.Res.Stored.Resources.Iron < lowestRes) { lowestRes = vill.Res.Stored.Resources.Iron; toUpgrade = BuildingEnum.IronMine; }
                if (distinctType == BuildingEnum.Cropland && vill.Res.Stored.Resources.Crop < lowestRes) { lowestRes = vill.Res.Stored.Resources.Crop; toUpgrade = BuildingEnum.Cropland; }
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

        #endregion
    }
}
