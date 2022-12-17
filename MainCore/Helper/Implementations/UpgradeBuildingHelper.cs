using FluentResults;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations
{
    public class UpgradeBuildingHelper : IUpgradeBuildingHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;

        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
        }

        public Result<PlanTask> NextBuildingTask(int accountId, int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            if (tasks.Count == 0)
            {
                return Result.Fail("Empty task queue");
            }
            using var context = _contextFactory.CreateDbContext();
            var currentList = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
            var totalBuild = currentList.Count(x => x.Level != -1);

            if (totalBuild > 0)
            {
                var accountInfo = context.AccountsInfo.Find(accountId);
                var tribe = accountInfo.Tribe;
                var hasPlusAccount = accountInfo.HasPlusAccount;
                var setting = context.VillagesSettings.Find(villageId);

                var maxBuild = 1;
                if (hasPlusAccount) maxBuild++;
                if (tribe == TribeEnums.Romans && !setting.IsIgnoreRomanAdvantage) maxBuild++;
                if (totalBuild == maxBuild)
                {
                    return Result.Fail("Amount of currently building is equal with maximum building can build in same time");
                }

                if (tribe == TribeEnums.Romans && !setting.IsIgnoreRomanAdvantage && maxBuild - totalBuild == 1)
                {
                    var numRes = currentList.Count(x => x.Type.IsResourceField());
                    var numInfra = totalBuild - numRes;

                    if (numRes > numInfra)
                    {
                        var freeCrop = context.VillagesResources.Find(villageId).FreeCrop;
                        if (freeCrop <= 5)
                        {
                            return Result.Fail("Cannot build because of lack of freecrop ( < 6 )");
                        }
                        return GetFirstInfrastructureTask(villageId);
                    }
                    else if (numInfra > numRes)
                    {
                        // no need check free crop, there is magic make sure this always choose crop
                        // jk, because of how we check free crop later, first res task is always crop
                        return GetFirstResTask(villageId);
                    }
                    // if same means 1 R and 1 I already, 1 ANY will be choose below
                }
            }

            return GetFirstTask(villageId);
        }

        public PlanTask ExtractResField(int villageId, PlanTask buildingTask)
        {
            List<VillageBuilding> buildings = null; // Potential buildings to be upgraded next
            using var context = _contextFactory.CreateDbContext();
            switch (buildingTask.ResourceType)
            {
                case ResTypeEnums.AllResources:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).Where(x => x.Type == BuildingEnums.Woodcutter || x.Type == BuildingEnums.ClayPit || x.Type == BuildingEnums.IronMine || x.Type == BuildingEnums.Cropland).ToList();
                    break;

                case ResTypeEnums.ExcludeCrop:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).Where(x => x.Type == BuildingEnums.Woodcutter || x.Type == BuildingEnums.ClayPit || x.Type == BuildingEnums.IronMine).ToList();
                    break;

                case ResTypeEnums.OnlyCrop:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).Where(x => x.Type == BuildingEnums.Cropland).ToList();
                    break;
            }

            foreach (var b in buildings)
            {
                if (b.IsUnderConstruction)
                {
                    var levelUpgrading = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Count(x => x.Location == b.Id);
                    b.Level += (byte)levelUpgrading;
                }
            }
            buildings = buildings.Where(b => b.Level < buildingTask.Level).ToList();

            if (buildings.Count == 0) return null;

            var building = buildings.OrderBy(x => x.Level).FirstOrDefault();

            return new()
            {
                Type = PlanTypeEnums.General,
                Level = building.Level + 1,
                Building = building.Type,
                Location = building.Id,
            };
        }

        public void RemoveFinishedCB(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var tasksDone = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Where(x => x.CompleteTime <= DateTime.Now);

            if (!tasksDone.Any()) return;

            foreach (var taskDone in tasksDone)
            {
                var building = context.VillagesBuildings.Find(villageId, taskDone.Location);
                if (building == null)
                {
                    building = context.VillagesBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == taskDone.Type);
                    if (building == null) continue;
                }

                if (building.Level < taskDone.Level) building.Level = taskDone.Level;

                taskDone.Type = 0;
                taskDone.Level = -1;
                taskDone.CompleteTime = DateTime.MaxValue;
            }
            context.SaveChanges();
        }

        private PlanTask GetFirstResTask(int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            var task = tasks.FirstOrDefault(x => x.Type == PlanTypeEnums.ResFields || x.Building.IsResourceField());
            return task;
        }

        private PlanTask GetFirstInfrastructureTask(int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            var infrastructureTasks = tasks.Where(x => x.Type == PlanTypeEnums.General && !x.Building.IsResourceField());
            var task = infrastructureTasks.FirstOrDefault(x => IsInfrastructureTaskVaild(villageId, x));
            return task;
        }

        private PlanTask GetFirstTask(int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            foreach (var task in tasks)
            {
                if (task.Type != PlanTypeEnums.General) return task;
                if (task.Building.IsResourceField()) return task;
                if (IsInfrastructureTaskVaild(villageId, task)) return task;
            }
            return null;
        }

        private bool IsInfrastructureTaskVaild(int villageId, PlanTask planTask)
        {
            (_, var prerequisiteBuildings) = planTask.Building.GetPrerequisiteBuildings();
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).ToList();
            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var building = buildings.FirstOrDefault(x => x.Type == prerequisiteBuilding.Building);
                if (building is null) return false;
                if (building.Level < prerequisiteBuilding.Level) return false;
            }
            return true;
        }
    }
}