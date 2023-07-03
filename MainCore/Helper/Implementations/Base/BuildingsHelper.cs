using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
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

            var buildings = new List<BuildingEnums>();
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                if (CanBuild(villageId, tribe, i)) buildings.Add(i);
            }
            return buildings;
        }

        public bool CanBuild(int villageId, TribeEnums tribe, BuildingEnums building)
        {
            if (IsExists(villageId, building))
            {
                return building switch
                {
                    BuildingEnums.Warehouse => IsBuildingAboveLevel(villageId, BuildingEnums.Warehouse, 20),
                    BuildingEnums.Granary => IsBuildingAboveLevel(villageId, BuildingEnums.Granary, 20),
                    BuildingEnums.GreatWarehouse => IsBuildingAboveLevel(villageId, BuildingEnums.GreatWarehouse, 20),
                    BuildingEnums.GreatGranary => IsBuildingAboveLevel(villageId, BuildingEnums.GreatGranary, 20),
                    BuildingEnums.Trapper => IsBuildingAboveLevel(villageId, BuildingEnums.Trapper, 20),
                    BuildingEnums.Cranny => IsBuildingAboveLevel(villageId, BuildingEnums.Cranny, 10),
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
                        if (IsAutoCropFieldAboveLevel(villageId, prerequisite.Level)) return true;
                    }
                    else
                    {
                        if (IsAutoResourceFieldAboveLevel(villageId, prerequisite.Level)) return true;
                    }
                }
                if (!IsBuildingAboveLevel(villageId, prerequisite.Building, prerequisite.Level)) return false;
            }
            return true;
        }

        public bool IsExists(int villageId, BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building);
            if (c) return true;
            var q = _planManager.GetList(villageId).Any(x => x.Building == building);
            if (q) return true;
            return false;
        }

        public bool IsBuildingAboveLevel(int villageId, BuildingEnums building, int level)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && level <= x.Level);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && level <= x.Level);
            if (c) return true;
            var q = _planManager.GetList(villageId).Any(x => x.Building == building && level <= x.Level);
            if (q) return true;
            return false;
        }

        public bool IsAutoResourceFieldAboveLevel(int villageId, int level)
        {
            return _planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.ExcludeCrop) && level <= x.Level);
        }

        public bool IsAutoCropFieldAboveLevel(int villageId, int level)
        {
            return _planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.OnlyCrop) && level <= x.Level);
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