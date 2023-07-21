using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class BuildingsHelper : IBuildingsHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;

        public BuildingsHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
        }

        public int GetDorf(int index) => index < 19 ? 1 : 2;

        public List<BuildingEnums> GetCanBuild(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(villageId);
            var tribe = context.AccountsInfo.Find(village.AccountId).Tribe;

            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).ToList();
            var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
            var queueBuildings = _planManager.GetList(villageId);

            var canBuildings = new List<BuildingEnums>();
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                if (CanBuild(i, tribe, buildings, currentlyBuildings, queueBuildings))
                {
                    canBuildings.Add(i);
                }
            }

            return canBuildings;
        }

        private static bool CanBuild(BuildingEnums building, TribeEnums tribe, List<VillageBuilding> buildings, List<VillCurrentBuilding> currentlyBuildings, List<PlanTask> queueBuildings)
        {
            if (IsExists(building, buildings, currentlyBuildings, queueBuildings))
            {
                return building switch
                {
                    BuildingEnums.Warehouse or BuildingEnums.Granary or BuildingEnums.GreatWarehouse or BuildingEnums.GreatGranary or BuildingEnums.Trapper => IsBuildingAboveLevel(building, 20, buildings, currentlyBuildings, queueBuildings),
                    BuildingEnums.Cranny => IsBuildingAboveLevel(building, 10, buildings, currentlyBuildings, queueBuildings),
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
                        if (IsAutoCropFieldAboveLevel(prerequisite.Level, queueBuildings)) return true;
                    }
                    else
                    {
                        if (IsAutoResourceFieldAboveLevel(prerequisite.Level, queueBuildings)) return true;
                    }
                }
                if (!IsBuildingAboveLevel(prerequisite.Building, prerequisite.Level, buildings, currentlyBuildings, queueBuildings)) return false;
            }
            return true;
        }

        private static bool IsExists(BuildingEnums building, List<VillageBuilding> buildings, List<VillCurrentBuilding> currentlyBuildings, List<PlanTask> queueBuildings)
        {
            var b = buildings.Any(x => x.Type == building);
            if (b) return true;
            var c = currentlyBuildings.Any(x => x.Type == building);
            if (c) return true;
            var q = queueBuildings.Any(x => x.Building == building);
            if (q) return true;
            return false;
        }

        private static bool IsBuildingAboveLevel(BuildingEnums building, int level, List<VillageBuilding> buildings, List<VillCurrentBuilding> currentlyBuildings, List<PlanTask> queueBuildings)
        {
            var b = buildings.Any(x => x.Type == building && x.Level >= level);
            if (b) return true;
            var c = currentlyBuildings.Any(x => x.Type == building && x.Level >= level);
            if (c) return true;
            var q = queueBuildings.Any(x => x.Building == building && x.Level >= level);
            if (q) return true;
            return false;
        }

        private static bool IsAutoResourceFieldAboveLevel(int level, List<PlanTask> queueBuildings)
        {
            return queueBuildings.Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.ExcludeCrop) && x.Level >= level);
        }

        private static bool IsAutoCropFieldAboveLevel(int level, List<PlanTask> queueBuildings)
        {
            return queueBuildings.Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.OnlyCrop) && x.Level >= level);
        }

        public int GetDorf(BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Woodcutter or BuildingEnums.ClayPit or BuildingEnums.IronMine or BuildingEnums.Cropland => 1,
                _ => 2,
            };
        }

        public bool IsTaskComplete(int villageId, PlanTask task)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId);
            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0);
            return _planManager.IsTaskComplete(task, buildings, currentBuildings);
        }

        public Result<bool> IsPrerequisiteAvailable(int villageId, PlanTask task)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId);
            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0);

            var (_, prerequisiteBuildings) = task.Building.GetPrerequisiteBuildings();

            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var isBuilt = buildings.Any(x => x.Type == prerequisiteBuilding.Building && x.Level >= prerequisiteBuilding.Level);
                if (!isBuilt)
                {
                    var isBuilding = currentBuildings.Any(x => x.Type == prerequisiteBuilding.Building && x.Level >= prerequisiteBuilding.Level);
                    if (!isBuilding) return false;
                    return Result.Fail(BuildingQueue.PrerequisiteInQueue(task));
                }
            }
            return true;
        }

        public Result<bool> IsMultipleReady(int villageId, PlanTask task)
        {
            if (!task.Building.IsMultipleAllow()) return true;
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId && x.Type == task.Building);
            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0 && x.Type == task.Building);

            if (!buildings.Any()) return true; // first building

            var highestLevelBuilding = buildings.OrderByDescending(x => x.Level).FirstOrDefault();
            if (highestLevelBuilding.Id == task.Location) return true;

            if (highestLevelBuilding.Level < task.Building.GetMaxLevel())
            {
                var isBuilding = currentBuildings.Any(x => x.Level == task.Building.GetMaxLevel());
                if (!isBuilding) return false;
                return Result.Fail(BuildingQueue.MultipleInQueue(task));
            }

            return true;
        }
    }
}