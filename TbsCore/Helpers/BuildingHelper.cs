using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.BuildingModels;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Files.Helpers
{
    public static class BuildingHelper
    {
        public static void AddBuildingTask(Account acc, Village vill, BuildingTask task, bool bottom = true)
        {
            if (task.BuildingId == null && task.TaskType == BuildingType.General)
            {
                //Check if bot has any space to build new buildings, otherwise return
                if (!FindBuildingId(vill, task)) return;
            }
            if (bottom) vill.Build.Tasks.Add(task);
            else vill.Build.Tasks.Insert(0, task);

            if (acc.Wb != null) ReStartBuilding(acc, vill);
        }

        /// <summary>
        /// Finds appropriate building space/site for the building
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="task">Building task</param>
        /// <returns>True if successful</returns>
        public static bool FindBuildingId(Village vill, BuildingTask task)
        {
            if (task.TaskType != BuildingType.General) return true; //auto field upgrade/demolish task, no need for Id
            var FreePlaces = vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.Site).ToList();
            bool FreePlace = false;
            foreach (var freePlc in FreePlaces)
            {
                if (!vill.Build.Tasks.Any(x => x.BuildingId == freePlc.Id)) FreePlace = true;
            }

            var ExistingBuilding = vill.Build.Buildings.FirstOrDefault(x => x.Type == task.Building);
            if (ExistingBuilding != null && !task.ConstructNew)
            {
                task.BuildingId = ExistingBuilding.Id;
                return true;
            }

            var ExistingBuildingTask = vill.Build.Tasks.FirstOrDefault(x => x.Building == task.Building && x.BuildingId != null);
            if (ExistingBuildingTask != null && !task.ConstructNew)
            {
                task.BuildingId = ExistingBuildingTask.BuildingId;
                return true;
            }

            if (!FreePlace) return false; //there is no space in the village to construct a new building

            byte id;
            do
            {
                Random ran = new Random();
                id = (byte)ran.Next(19, 39);
            } //search for available building id;
            while (vill.Build.Buildings.FirstOrDefault(x => x.Id == id).Type != Classificator.BuildingEnum.Site || vill.Build.Tasks.FirstOrDefault(x => x.BuildingId == id) != null);
            //if new village, you should return build.php?id=25&category=3
            task.BuildingId = id;
            return true;
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

            var nextExecution = DateTime.Now.AddSeconds(5);
            var lastCB = vill.Build.CurrentlyBuilding.LastOrDefault();

            var maxBuildings = 1;
            if (acc.AccInfo.PlusAccount) maxBuildings++;
            if (acc.AccInfo.Tribe == Classificator.TribeEnum.Romans) maxBuildings++;

            if (lastCB != null && lastCB.Duration > nextExecution && vill.Build.CurrentlyBuilding.Count >= maxBuildings) nextExecution = lastCB.Duration;

            var building = new UpgradeBuilding()
            {
                Vill = vill,
                ExecuteAt = nextExecution,
            };
            TaskExecutor.AddTask(acc, building);
        }

        public static void ReStartDemolishing(Account acc, Village vill)
        {
            if (vill.Build.DemolishTasks.Count > 0)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new DemolishBuilding() { Vill = vill, ExecuteAt = DateTime.Now.AddSeconds(10) });
            }
        }

        public static bool BuildingRequirementsAreMet(Classificator.BuildingEnum building, Village vill, Classificator.TribeEnum tribe) //check if user can construct this building
        {
            bool exists = (vill.Build.Buildings.FirstOrDefault(x => x.Type == building) != null); //there is already a building of this type in the vill
            if (exists)
            {
                //check cranny/warehouse/grannary/trapper/GG/GW
                switch (building)
                {
                    case Classificator.BuildingEnum.Warehouse: return BuildingIsOnLevel(Classificator.BuildingEnum.Warehouse, 20, vill);
                    case Classificator.BuildingEnum.Granary: return BuildingIsOnLevel(Classificator.BuildingEnum.Granary, 20, vill);
                    case Classificator.BuildingEnum.GreatWarehouse: return BuildingIsOnLevel(Classificator.BuildingEnum.GreatWarehouse, 20, vill);
                    case Classificator.BuildingEnum.GreatGranary: return BuildingIsOnLevel(Classificator.BuildingEnum.GreatGranary, 20, vill);
                    case Classificator.BuildingEnum.Trapper: return BuildingIsOnLevel(Classificator.BuildingEnum.Trapper, 20, vill);
                    case Classificator.BuildingEnum.Cranny: return BuildingIsOnLevel(Classificator.BuildingEnum.Cranny, 10, vill);
                    default: return false;
                }
            }

            //check for prerequisites for this building
            (var ReqTribe, var Prerequisites) = GetBuildingPrerequisites(building);
            if (ReqTribe != Classificator.TribeEnum.Any && ReqTribe != tribe) return false;
            //if we either already have this building OR is on our build task list, requirements are met.
            foreach (var prerequisite in Prerequisites)
            {
                if (!vill.Build.Buildings.Any(x => x.Level >= prerequisite.Level && x.Type == prerequisite.Building)
                    && !vill.Build.Tasks.Any(x => x.Level >= prerequisite.Level && x.Building == prerequisite.Building)) return false;
            }
            return true;
        }

        private static bool BuildingIsOnLevel(Classificator.BuildingEnum building, int lvl, Village vill)
        {
            //if there already is a building on specific level or there is a task for this building
            return (vill.Build.Buildings.FirstOrDefault(x => x.Level == lvl && x.Type == building) != null || vill.Build.Tasks.FirstOrDefault(x => x.Level == lvl && x.Building == building) != null);
        }

        public static void RemoveFinishedCB(Village vill)
        {
            for (int i = 0; i < vill.Build.CurrentlyBuilding.Count; i++)
            {
                if (vill.Build.CurrentlyBuilding[i].Duration.AddMilliseconds(500) < DateTime.Now)
                {
                    vill.Build.CurrentlyBuilding.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Used by building task to get the url for navigation
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="task">BotTask</param>
        /// <returns></returns>
        public static string GetUrlForBuilding(Village vill, BuildingTask task)
        {
            switch (task.TaskType)
            {
                case BuildingType.General:
                    return GetUrlGeneralTask(vill, task);
                case BuildingType.AutoUpgradeResFields:
                    return GetUrlAutoResFields(vill, task);
            }
            return null;
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
                            //if ((this.Building == Classificator.Type.Residence) && (this.Level == 10 || this.Level == 20)) TrainSettlers(vill, acc, 0);
                            return true;
                        }
                        return false; //this building doest exist yet!
                    }
                    if (building.Level >= task.Level || (building.Level + 1 == task.Level && building.UnderConstruction))
                    {
                        //if (this.Building == Classificator.Type.Residence) TrainSettlers(vill, acc, 30);
                        return true;
                    }
                    return false;

                case BuildingType.AutoUpgradeResFields:
                    if (vill.Build.Buildings[0].Type == Classificator.BuildingEnum.Site) return false; //for new villages that are not checked yet
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
        private static string GetUrlGeneralTask(Village vill, BuildingTask task)
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
                        return null;
                    }
                }
            }

            var url = task.BuildingId.ToString();
            // If there is no building in that space currently, construct a new building
            if (vill.Build.Buildings.Any(x => x.Type == Classificator.BuildingEnum.Site && x.Id == task.BuildingId))
            {
                url += "&category=" + (int)BuildingHelper.GetBuildingsCategory(task.Building);
            }
            return url;
        }
        public static string GetUrlAutoResFields(Village vill, BuildingTask task)
        {
            List<Models.ResourceModels.Building> buildings; // Potential buildings to be upgraded next
            switch (task.ResourceType)
            {
                case ResTypeEnum.AllResources:
                    buildings = vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.Woodcutter || x.Type == Classificator.BuildingEnum.ClayPit || x.Type == Classificator.BuildingEnum.IronMine || x.Type == Classificator.BuildingEnum.Cropland).ToList();
                    break;
                case ResTypeEnum.ExcludeCrop:
                    buildings = vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.Woodcutter || x.Type == Classificator.BuildingEnum.ClayPit || x.Type == Classificator.BuildingEnum.IronMine).ToList();
                    break;
                case ResTypeEnum.OnlyCrop:
                    buildings = vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.Cropland).ToList();
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

        internal static void RemoveDuplicateBuildingTasks(Village vill)
        {
            if (vill.Build.Tasks.Count == 0) return;
            vill.Build.Tasks = vill.Build.Tasks.Distinct().ToList();
        }

        public enum BuildingType
        {
            General,
            AutoUpgradeResFields
        }
        public enum BuildingDorf
        {
            Dorf1,
            Dorf2
        }


        /// <summary>
        /// Adds all building prerequisites for this building if they do not exist yet. After this you should call RemoveDuplicates().
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="vill"></param>
        /// <param name="building"></param>
        /// <returns>Whether we have all prerequisite buildings</returns>
        public static bool AddBuildingPrerequisites(Account acc, Village vill, Classificator.BuildingEnum building)
        {
            (var tribe, var prereqs) = GetBuildingPrerequisites(building);
            if (acc.AccInfo.Tribe != tribe) return false;
            if (prereqs.Count == 0) return true;
            var ret = true;
            foreach (var prereq in prereqs)
            {
                var prereqBuilding = vill.Build.Buildings.FirstOrDefault(x =>
                    x.Type == prereq.Building &&
                    x.Level >= prereq.Level
                    );
                // Prerequired building already exists
                if (prereqBuilding != null) continue;
                // Check if we have its prerequisites
                AddBuildingPrerequisites(acc, vill, prereq.Building);
                AddBuildingTask(acc, vill, new BuildingTask()
                {
                    Building = prereq.Building,
                    Level = prereq.Level,
                    TaskType = BuildingType.General
                });
                ret = false;
            }
            return ret;
        }
        public static bool IsResourceField(Classificator.BuildingEnum building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }
        public static void RemoveCompletedTasks(Village vill, Account acc)
        {
            foreach (var task in vill.Build.Tasks.ToList())
            {
                if (IsTaskCompleted(vill, acc, task))
                    vill.Build.Tasks.Remove(task);
            }
        }

        public enum BuildingCategoryEnum
        {
            Infrastructure = 1,
            Military = 2,
            Resources = 3
        }
        /// <summary>
        /// Buildings that are always build in the same spot
        /// </summary>
        //public static Classificator.BuildingEnum[] StaticBuildings = new Classificator.BuildingEnum[] {
        //    Classificator.BuildingEnum.Wall,
        //    Classificator.BuildingEnum.MakeshiftWall,
        //    Classificator.BuildingEnum.Palisade,
        //    Classificator.BuildingEnum.StoneWall,
        //    Classificator.BuildingEnum.EarthWall,
        //    Classificator.BuildingEnum.CityWall,
        //    Classificator.BuildingEnum.RallyPoint,
        //    Classificator.BuildingEnum.WW
        //};
        public static BuildingCategoryEnum GetBuildingsCategory(Classificator.BuildingEnum building)
        {
            switch (building)
            {
                case Classificator.BuildingEnum.GrainMill:
                case Classificator.BuildingEnum.Sawmill:
                case Classificator.BuildingEnum.Brickyard:
                case Classificator.BuildingEnum.IronFoundry:
                case Classificator.BuildingEnum.Bakery:
                    return BuildingCategoryEnum.Resources;
                case Classificator.BuildingEnum.RallyPoint:
                case Classificator.BuildingEnum.EarthWall:
                case Classificator.BuildingEnum.CityWall:
                case Classificator.BuildingEnum.MakeshiftWall:
                case Classificator.BuildingEnum.StoneWall:
                case Classificator.BuildingEnum.Palisade:
                case Classificator.BuildingEnum.Barracks:
                case Classificator.BuildingEnum.HerosMansion:
                case Classificator.BuildingEnum.Academy:
                case Classificator.BuildingEnum.Smithy:
                case Classificator.BuildingEnum.Stable:
                case Classificator.BuildingEnum.GreatBarracks:
                case Classificator.BuildingEnum.GreatStable:
                case Classificator.BuildingEnum.Workshop:
                case Classificator.BuildingEnum.TournamentSquare:
                    return BuildingCategoryEnum.Military;
                default:
                    return BuildingCategoryEnum.Infrastructure;
            }
        }

        public static (Classificator.TribeEnum, List<Prerequisite>) GetBuildingPrerequisites(Classificator.BuildingEnum building)
        {
            Classificator.TribeEnum tribe = Classificator.TribeEnum.Any;
            var ret = new List<Prerequisite>();
            switch (building)
            {
                case Classificator.BuildingEnum.Woodcutter:
                case Classificator.BuildingEnum.ClayPit:
                case Classificator.BuildingEnum.IronMine:
                case Classificator.BuildingEnum.Cropland:
                    break;
                case Classificator.BuildingEnum.Sawmill:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Woodcutter, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    break;
                case Classificator.BuildingEnum.Brickyard:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.ClayPit, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    break;
                case Classificator.BuildingEnum.IronFoundry:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.IronMine, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    break;
                case Classificator.BuildingEnum.GrainMill:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Cropland, Level = 5 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    break;
                case Classificator.BuildingEnum.Bakery:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Cropland, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.GrainMill, Level = 5 });
                    break;
                case Classificator.BuildingEnum.Warehouse:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 1 });
                    break;
                case Classificator.BuildingEnum.Granary:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 1 });
                    break;
                case Classificator.BuildingEnum.Blacksmith:
                    //DOESN'T EXIST ANYMORE
                    tribe = Classificator.TribeEnum.Nature; //Just a dirty hack, since user can't be Nature, he can't build Blacksmith
                    break;
                case Classificator.BuildingEnum.Smithy:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Academy, Level = 1 });
                    break;
                case Classificator.BuildingEnum.TournamentSquare:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.RallyPoint, Level = 15 });
                    break;
                case Classificator.BuildingEnum.MainBuilding:
                    break;
                case Classificator.BuildingEnum.RallyPoint:
                    break;
                case Classificator.BuildingEnum.Marketplace:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Warehouse, Level = 1 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Granary, Level = 1 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 3 });
                    break;
                case Classificator.BuildingEnum.Embassy:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 1 });
                    break;
                case Classificator.BuildingEnum.Barracks:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.RallyPoint, Level = 1 });
                    break;
                case Classificator.BuildingEnum.Stable:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Smithy, Level = 3 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Academy, Level = 5 });
                    break;
                case Classificator.BuildingEnum.Workshop:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    break;
                case Classificator.BuildingEnum.Academy:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Barracks, Level = 3 });
                    break;
                case Classificator.BuildingEnum.Cranny:
                    break;
                case Classificator.BuildingEnum.TownHall:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 10 });
                    break;
                case Classificator.BuildingEnum.Residence:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 }); //no palace!
                    break;
                case Classificator.BuildingEnum.Palace:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 }); //no residence!
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Embassy, Level = 1 });
                    break;
                case Classificator.BuildingEnum.Treasury:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 10 });
                    break;
                case Classificator.BuildingEnum.TradeOffice:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Stable, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Marketplace, Level = 20 });
                    break;
                case Classificator.BuildingEnum.GreatBarracks:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Barracks, Level = 20 }); //not capital!
                    break;
                case Classificator.BuildingEnum.GreatStable:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Stable, Level = 20 }); //not capital
                    break;
                case Classificator.BuildingEnum.CityWall:
                    tribe = Classificator.TribeEnum.Romans;
                    break;
                case Classificator.BuildingEnum.EarthWall:
                    tribe = Classificator.TribeEnum.Teutons;
                    break;
                case Classificator.BuildingEnum.Palisade:
                    tribe = Classificator.TribeEnum.Gauls;
                    break;
                case Classificator.BuildingEnum.StonemasonsLodge:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 }); //capital
                    break;
                case Classificator.BuildingEnum.Brewery:
                    tribe = Classificator.TribeEnum.Teutons;
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Granary, Level = 20 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.RallyPoint, Level = 10 });
                    break;
                case Classificator.BuildingEnum.Trapper:
                    tribe = Classificator.TribeEnum.Gauls;
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.RallyPoint, Level = 1 });
                    break;
                case Classificator.BuildingEnum.HerosMansion:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 3 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.RallyPoint, Level = 1 });
                    break;
                case Classificator.BuildingEnum.GreatWarehouse:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 10 }); //art/ww vill
                    break;
                case Classificator.BuildingEnum.GreatGranary:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 10 }); //art/ww vill
                    break;
                case Classificator.BuildingEnum.WW: //ww vill
                    tribe = Classificator.TribeEnum.Nature; //Just a dirty hack, since user can't be Nature, he can't constuct WW.
                    break;
                case Classificator.BuildingEnum.HorseDrinkingTrough:
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.RallyPoint, Level = 10 });
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.Stable, Level = 20 });
                    tribe = Classificator.TribeEnum.Romans;
                    break;
                case Classificator.BuildingEnum.StoneWall:
                    tribe = Classificator.TribeEnum.Egyptians;
                    break;
                case Classificator.BuildingEnum.MakeshiftWall:
                    tribe = Classificator.TribeEnum.Huns;
                    break;
                case Classificator.BuildingEnum.CommandCenter: //no res/palace
                    tribe = Classificator.TribeEnum.Huns;
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
                    break;
                case Classificator.BuildingEnum.Waterworks:
                    tribe = Classificator.TribeEnum.Egyptians;
                    ret.Add(new Prerequisite() { Building = Classificator.BuildingEnum.HerosMansion, Level = 10 });
                    break;
                default: break;
            }
            return (tribe, ret);
        }

        #region Functions for auto-building resource fields
        private static Models.ResourceModels.Building FindLowestLevelBuilding(List<Models.ResourceModels.Building> buildings)
        {
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
            Classificator.BuildingEnum toUpgrade = Classificator.BuildingEnum.Cropland;

            foreach (var distinctType in distinct)
            {
                if (distinctType == Classificator.BuildingEnum.Woodcutter && vill.Res.Production.Wood < lowestProd) { lowestProd = vill.Res.Production.Wood; toUpgrade = Classificator.BuildingEnum.Woodcutter; }
                if (distinctType == Classificator.BuildingEnum.ClayPit && vill.Res.Production.Clay < lowestProd) { lowestProd = vill.Res.Production.Clay; toUpgrade = Classificator.BuildingEnum.ClayPit; }
                if (distinctType == Classificator.BuildingEnum.IronMine && vill.Res.Production.Iron < lowestProd) { lowestProd = vill.Res.Production.Iron; toUpgrade = Classificator.BuildingEnum.IronMine; }
                if (distinctType == Classificator.BuildingEnum.Cropland && vill.Res.Production.Crop < lowestProd) { lowestProd = vill.Res.Production.Crop; toUpgrade = Classificator.BuildingEnum.Cropland; }
            }
            return FindLowestLevelBuilding(buildings.Where(x => x.Type == toUpgrade).ToList());
        }
        private static Models.ResourceModels.Building GetLowestRes(List<Models.ResourceModels.Building> buildings, Village vill)
        {
            //get distinct field types
            var distinct = buildings.Select(x => x.Type).Distinct().ToList();
            long lowestRes = long.MaxValue;
            Classificator.BuildingEnum toUpgrade = Classificator.BuildingEnum.Cropland;

            foreach (var distinctType in distinct)
            {
                if (distinctType == Classificator.BuildingEnum.Woodcutter && vill.Res.Stored.Resources.Wood < lowestRes) { lowestRes = vill.Res.Stored.Resources.Wood; toUpgrade = Classificator.BuildingEnum.Woodcutter; }
                if (distinctType == Classificator.BuildingEnum.ClayPit && vill.Res.Stored.Resources.Clay < lowestRes) { lowestRes = vill.Res.Stored.Resources.Clay; toUpgrade = Classificator.BuildingEnum.ClayPit; }
                if (distinctType == Classificator.BuildingEnum.IronMine && vill.Res.Stored.Resources.Iron < lowestRes) { lowestRes = vill.Res.Stored.Resources.Iron; toUpgrade = Classificator.BuildingEnum.IronMine; }
                if (distinctType == Classificator.BuildingEnum.Cropland && vill.Res.Stored.Resources.Crop < lowestRes) { lowestRes = vill.Res.Stored.Resources.Crop; toUpgrade = Classificator.BuildingEnum.Cropland; }
            }
            return FindLowestLevelBuilding(buildings.Where(x => x.Type == toUpgrade).ToList());
        }
        private static bool CheckExcludeCrop(Village vill, BuildingTask task)
        {
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.Woodcutter))
            {
                if (res.Level < task.Level)
                {
                    if (res.Level == task.Level - 1 && res.UnderConstruction) continue; //if on level below + is being upgraded to that lvl
                    return false;
                }
            }
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.ClayPit))
            {
                if (res.Level < task.Level)
                {
                    if (res.Level == task.Level - 1 && res.UnderConstruction) continue; //if on level below + is being upgraded to that lvl
                    return false;
                }
            }
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.IronMine))
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
            foreach (var res in vill.Build.Buildings.Where(x => x.Type == Classificator.BuildingEnum.Cropland))
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
