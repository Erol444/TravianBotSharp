using System;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;
using static TbsCore.Helpers.Classificator;
using TbsCore.TravianData;
using System.Collections.Generic;
using TbsCore.Tasks;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Helpers
{
    /// <summary>
    /// Helper class for the UpgradeBuilding BotTask
    /// </summary>
    public class UpgradeBuildingHelper
    {
        public static BuildingTask NextBuildingTask(Account acc, Village vill)
        {
            if (vill.Build.Tasks.Count == 0)
            {
                acc.Logger.Information("Building queue empty.");
                return null;
            }

            RemoveFinishedCB(vill);

            var totalBuild = vill.Build.CurrentlyBuilding.Count;
            if (totalBuild > 0)
            {
                var maxBuild = 1;
                if (acc.AccInfo.PlusAccount) maxBuild++;
                if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild++;
                if (totalBuild == maxBuild)
                {
                    acc.Logger.Information("Amount of currently building is equal with maximum building can build in same time");
                    return null;
                }

                if (maxBuild - totalBuild == 1)
                {
                    if (acc.AccInfo.Tribe == TribeEnum.Romans ||
                        (acc.AccInfo.PlusAccount && acc.AccInfo.ServerVersion == ServerVersionEnum.TTwars))
                    {
                        var numRes = vill.Build.CurrentlyBuilding.Count(x => BuildingHelper.IsResourceField(x.Building));
                        var numInfra = totalBuild - numRes;

                        if (numRes > numInfra)
                        {
                            if (vill.Res.FreeCrop <= 5) return null;
                            return GetFirstInfrastructureTask(acc, vill);
                        }
                        else if (numInfra > numRes)
                        {
                            // no need check free crop, there is magic make sure this always choose crop
                            return GetFirstResTask(acc, vill);
                        }
                        // if same means 1 R and 1 I already, 1 ANY will be choose below
                    }
                }
            }

            return GetFirstTask(acc, vill);
        }

        public static bool CheckGeneralTaskBuildPlace(Village vill, BuildingTask task)
        {
            if (task.BuildingId != null && // we already find a place to build it
                (vill.Build.Buildings.First(x => x.Id == task.BuildingId).Type == task.Building || // place we choose has same building type
                vill.Build.Buildings.First(x => x.Id == task.BuildingId).Type == BuildingEnum.Site)) // place we choose is empty slot
            {
                return true;
            }
            // find building has same type
            var targetBuilding = vill.Build.Buildings.FirstOrDefault(x => x.Type == task.Building);
            if (targetBuilding != null && !task.ConstructNew) // we found building and this task is not build new one
            {
                task.BuildingId = targetBuilding.Id;
                return true;
            }

            return FindPlaceToBuild(vill, task);
        }

        public static void AddResFields(Account acc, Village vill, BuildingTask task)
        {
            List<Building> buildings = null; // Potential buildings to be upgraded next
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
            }

            buildings = buildings.Where(b =>
            {
                var level = b.Level;
                if (b.UnderConstruction)
                {
                    var levelUpgrading = vill.Build.CurrentlyBuilding.Count(x => x.Location == b.Id);
                    level += (byte)levelUpgrading;
                }
                return task.Level > level;
            }).ToList();

            Building buildingToUpgrade = null;
            switch (task.BuildingStrategy)
            {
                case BuildingStrategyEnum.BasedOnLevel:
                    buildingToUpgrade = BuildingHelper.FindLowestLevelBuilding(buildings);
                    break;

                case BuildingStrategyEnum.BasedOnProduction:
                    buildingToUpgrade = BuildingHelper.GetLowestProduction(buildings, vill);
                    break;

                case BuildingStrategyEnum.BasedOnRes:
                    buildingToUpgrade = BuildingHelper.GetLowestRes(acc, vill, buildings);
                    break;
            }
            if (buildingToUpgrade == null)
            {
                vill.Build.Tasks.Remove(task);
                return;
            }

            AddBuildingTask(acc, vill, new BuildingTask()
            {
                TaskType = BuildingType.General,
                Building = buildingToUpgrade.Type,
                BuildingId = buildingToUpgrade.Id,
                Level = buildingToUpgrade.Level + 1,
            }, false);
        }

        private static bool IsValidTask(Account acc, Village vill, BuildingTask task)
        {
            // check complete
            if (IsTaskCompleted(vill, task))
            {
                acc.Logger.Warning($"{task.Building} - Level {task.Level} already completed. Removed");
                vill.Build.Tasks.Remove(task);
                return false;
            }
            // check space
            if (task.BuildingId == null && task.TaskType == BuildingType.General)
            {
                var found = FindPlaceToBuild(vill, task);
                //no space found for this building, remove the buildTask
                if (!found)
                {
                    acc.Logger.Warning($"No space for {task.Building}. Removed");
                    vill.Build.Tasks.Remove(task);
                    return false;
                }
            }

            // check prerequisite
            var prerequisite = AddBuildingPrerequisites(acc, vill, task.Building, false);
            if (!prerequisite) return false;

            return true;
        }

        private static BuildingTask GetFirstResTask(Account acc, Village vill)
        {
            do
            {
                var task = vill.Build.Tasks.FirstOrDefault(x =>
                x.TaskType == BuildingType.AutoUpgradeResFields || BuildingHelper.IsResourceField(x.Building));
                if (task == null) return null;
                if (!IsValidTask(acc, vill, task)) continue;

                return task;
            } while (vill.Build.Tasks.Count > 0);
            return null;
        }

        private static BuildingTask GetFirstInfrastructureTask(Account acc, Village vill)
        {
            do
            {
                var task = vill.Build.Tasks.FirstOrDefault(x => x.TaskType == BuildingType.General && !BuildingHelper.IsResourceField(x.Building));
                if (task == null) return null;
                if (!IsValidTask(acc, vill, task)) continue;

                return task;
            } while (vill.Build.Tasks.Count > 0);
            return null;
        }

        private static BuildingTask GetFirstTask(Account acc, Village vill)
        {
            do
            {
                var task = vill.Build.Tasks.FirstOrDefault();
                if (task == null) return null;
                if (!IsValidTask(acc, vill, task)) continue;
                return task;
            } while (vill.Build.Tasks.Count > 0);
            return null;
        }

        /// <summary>
        /// Finds appropriate building space/site for the building
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="task">Building task</param>
        /// <returns>True if successful</returns>
        private static bool FindPlaceToBuild(Village vill, BuildingTask task)
        {
            // we dont need to find place for res fileds
            if (task.TaskType == BuildingType.AutoUpgradeResFields) return true;

            // check building has same type
            var existingBuilding = vill.Build.Buildings.FirstOrDefault(x => x.Type == task.Building);

            // Only special buildings (warehouse, cranny, granary etc.) can have multiple
            // buildings of it's type and use ConstructNew option
            if (!BuildingsData.CanHaveMultipleBuildings(task.Building)) task.ConstructNew = false;

            // change to current building place instead of build new one
            if (existingBuilding != null && !task.ConstructNew)
            {
                task.BuildingId = existingBuilding.Id;
                return true;
            }

            // check in building queue, there is same building but has place already
            var ExistingBuildingTask = vill.Build.Tasks.FirstOrDefault(x => x.Building == task.Building && x.BuildingId != null);

            // change to building's place in queue instead of build new one
            if (ExistingBuildingTask != null && !task.ConstructNew)
            {
                task.BuildingId = ExistingBuildingTask.BuildingId;
                return true;
            }

            // now find place to build in empty slot
            var FreeSites = vill.Build.Buildings
                .Where(x => x.Type == BuildingEnum.Site && 19 <= x.Id && x.Id <= 39)
                .OrderBy(a => Guid.NewGuid()) // Shuffle the free sites
                .ToList();

            foreach (var FreeSite in FreeSites)
            {
                // check if there is any building in queue use this slot
                if (!vill.Build.Tasks.Any(x => x.BuildingId == FreeSite.Id))
                {
                    // Site is free and there's no building task that reserves it.
                    task.BuildingId = FreeSite.Id;
                    return true;
                }
            }
            return false;
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

            // Current lvl in bot's data
            var currentLvl = (int)upgrade.Level;

            RemoveFinishedCB(vill);

            // Current lvl in current building list
            var currentBuilding = vill.Build.CurrentlyBuilding.FirstOrDefault(x => x.Building == building);
            if (currentBuilding != null) currentLvl = currentBuilding.Level;

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
        /// Adds the building task to the village list of building tasks. Restarts BotTask UpgradeBuilding if needed.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="task">BuildingTask to add</param>
        /// <param name="bottom">Whether to insert the BuildingTask on the bottom of the list</param>
        /// <returns>Whether the method completed successfully</returns>
        public static bool AddBuildingTask(Account acc, Village vill, BuildingTask task, bool bottom = true, bool restart = true)
        {
            if (vill == null) return false;

            //check wall
            if (BuildingHelper.IsWall(task.Building))
            {
                var wall = BuildingsData.GetTribesWall(acc.AccInfo.Tribe);
                // check type
                if (task.Building != wall) task.Building = wall;
                // check position
                if (task.BuildingId != 40) task.BuildingId = 40;
            }
            // check rally point
            else if (task.Building == BuildingEnum.Site)
            {
                // check position
                if (task.BuildingId != 39) task.BuildingId = 39;
            }
            // other building
            else if (task.BuildingId == null ||
                     vill.Build.Buildings.Any(x => x.Id == task.BuildingId &&
                                                   x.Type != task.Building &&
                                                   x.Type != BuildingEnum.Site))
            {
                //Check if bot has any space to build new buildings, otherwise return
                if (!FindPlaceToBuild(vill, task)) return false;
            }

            // checking multiple building
            // you need at least one at level 20 before building other
            if (BuildingsData.CanHaveMultipleBuildings(task.Building))
            {
                if (task.ConstructNew)
                {
                    // Highest level building
                    var highestLvl = vill.Build
                        .Buildings
                        .Where(x => x.Type == task.Building)
                        .OrderByDescending(x => x.Level)
                        .FirstOrDefault();

                    if (highestLvl != null &&
                        highestLvl.Level != BuildingsData.MaxBuildingLevel(acc, task.Building))
                    {
                        task.BuildingId = highestLvl.Id;
                    }
                }
            }
            else if (!BuildingHelper.IsResourceField(task.Building))
            {
                var buildings = vill.Build.Buildings.Where(x => x.Type == task.Building);
                if (buildings.Count() > 0)
                {
                    var id = buildings.First().Id;
                    if (id != task.BuildingId)
                    {
                        task.BuildingId = id;
                    }
                }
            }

            if (bottom) vill.Build.Tasks.Add(task);
            else vill.Build.Tasks.Insert(0, task);

            if (restart) ReStartBuilding(acc, vill);
            return true;
        }

        public static void ReStartBuilding(Account acc, Village vill)
        {
            RemoveFinishedCB(vill);
            RemoveCompletedTasks(vill);
            var upgradeBuildingTask = acc.Tasks.FindTask(typeof(UpgradeBuilding), vill);
            if (upgradeBuildingTask == null)
            {
                acc.Tasks.Add(new UpgradeBuilding()
                {
                    Vill = vill,
                    ExecuteAt = DateTime.Now,
                }, true, vill);
                return;
            }

            upgradeBuildingTask.ExecuteAt = DateTime.Now;
            acc.Tasks.ReOrder();
        }

        /// <summary>
        /// Removes all complete building tasks
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="acc">Account</param>
        public static void RemoveCompletedTasks(Village vill) => vill.Build.Tasks.RemoveAll(task => IsTaskCompleted(vill, task));

        public static bool IsTaskCompleted(Village vill, BuildingTask task)
        {
            if (vill == null) return true;
            switch (task.TaskType)
            {
                case BuildingType.General:
                    {
                        var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId);

                        if (building == null)
                        {
                            building = vill.Build.Buildings.FirstOrDefault(x => x.Type == task.Building);
                            if (building == null) return false;
                        }

                        // Building is on / above desired level, task is completed
                        if (task.Level <= building.Level) return true;

                        // If the building is being upgraded to the desired level, task is complete
                        var cb = vill.Build
                            .CurrentlyBuilding
                            .OrderByDescending(x => x.Level)
                            .FirstOrDefault(x => x.Location == task.BuildingId);
                        if (cb != null && task.Level <= cb.Level) return true;
                        break;
                    }
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

        /// <summary>
        /// Remove all finished "currently building"
        /// </summary>
        /// <param name="vill"></param>
        /// <returns>Whether there were some tasks removed</returns>
        public static bool RemoveFinishedCB(Village vill)
        {
            var tasksDone = vill.Build
                .CurrentlyBuilding
                .Where(x => x.Duration < DateTime.Now)
                .ToList();

            if (tasksDone.Count == 0) return false;

            foreach (var taskDone in tasksDone)
            {
                var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == taskDone.Location);
                if (building == null)
                {
                    building = vill.Build.Buildings.FirstOrDefault(x => x.Type == taskDone.Building);

                    if (building == null) continue;
                }

                if (building.Level < taskDone.Level) building.Level = taskDone.Level;
                vill.Build.CurrentlyBuilding.Remove(taskDone);
            }
            return true;
        }

        /// <summary>
        /// Adds all building prerequisites for this building if they do not exist yet.
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="vill"></param>
        /// <param name="building"></param>
        /// <returns>Whether we have all prerequisite buildings</returns>
        public static bool AddBuildingPrerequisites(Account acc, Village vill, BuildingEnum building, bool bottom = true)
        {
            RemoveFinishedCB(vill);

            (var tribe, var prereqs) = BuildingsData.GetBuildingPrerequisites(building);
            if (acc.AccInfo.Tribe != tribe && tribe != TribeEnum.Any) return false;
            if (prereqs.Count == 0) return true;

            var ret = true;

            foreach (var prereq in prereqs)
            {
                var prereqBuilding = vill.Build.Buildings.FirstOrDefault(x => x.Type == prereq.Building && x.Level >= prereq.Level);
                if (prereqBuilding == null)
                {
                    var currentlyBuilding = vill.Build.CurrentlyBuilding.FirstOrDefault(x => x.Building == prereq.Building && x.Level >= prereq.Level);
                    if (currentlyBuilding == null)
                    {
                        if (bottom) // bottom add prereq first, building follow
                        {
                            AddBuildingPrerequisites(acc, vill, prereq.Building, bottom);
                            AddBuildingTask(acc, vill, new BuildingTask()
                            {
                                Building = prereq.Building,
                                Level = prereq.Level,
                                TaskType = BuildingType.General
                            }, bottom);
                        }
                        else //top, add building first, prereq follow,
                        {
                            AddBuildingTask(acc, vill, new BuildingTask()
                            {
                                Building = prereq.Building,
                                Level = prereq.Level,
                                TaskType = BuildingType.General
                            }, bottom);
                            AddBuildingPrerequisites(acc, vill, prereq.Building, bottom);
                        }
                    }
                    ret = false;
                }
            }
            return ret;
        }
    }
}