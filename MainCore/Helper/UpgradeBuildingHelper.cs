using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper
{
    public static class UpgradeBuildingHelper
    {
        public static PlanTask NextBuildingTask(AppDbContext context, IPlanManager planManager, ILogManager logManager, int accountId, int villageId)
        {
            var tasks = planManager.GetList(villageId);
            if (tasks.Count == 0)
            {
                logManager.Information(accountId, "Empty task queue");
                return null;
            }

            var currentList = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
            var totalBuild = currentList.Count(x => x.Level != -1);

            if (totalBuild > 0)
            {
                var accountInfo = context.AccountsInfo.Find(accountId);
                var tribe = accountInfo.Tribe;
                var hasPlusAccount = accountInfo.HasPlusAccount;

                var maxBuild = 1;
                if (hasPlusAccount) maxBuild++;
                if (tribe == TribeEnums.Romans) maxBuild++;
                if (totalBuild == maxBuild)
                {
                    logManager.Information(accountId, "Amount of currently building is equal with maximum building can build in same time");
                    return null;
                }

                if (tribe == TribeEnums.Romans && maxBuild - totalBuild == 1)
                {
                    var numRes = currentList.Count(x => x.Type.IsResourceField());
                    var numInfra = totalBuild - numRes;

                    if (numRes > numInfra)
                    {
                        var freeCrop = context.VillagesResources.Find(villageId).FreeCrop;
                        if (freeCrop <= 5)
                        {
                            logManager.Information(accountId, "Cannot build because of lack of freecrop");
                            return null;
                        }
                        return GetFirstInfrastructureTask(planManager, villageId);
                    }
                    else if (numInfra > numRes)
                    {
                        // no need check free crop, there is magic make sure this always choose crop
                        // jk, because of how we check free crop later, first res task is always crop
                        return GetFirstResTask(planManager, villageId);
                    }
                    // if same means 1 R and 1 I already, 1 ANY will be choose below
                }
            }

            return GetFirstTask(planManager, villageId);
        }

        public static PlanTask ExtractResField(AppDbContext context, int villageId, PlanTask buildingTask)
        {
            List<VillageBuilding> buildings = null; // Potential buildings to be upgraded next
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

            buildings.ForEach(b =>
            {
                if (b.IsUnderConstruction)
                {
                    var levelUpgrading = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Count(x => x.Location == b.Id);
                    b.Level += (byte)levelUpgrading;
                }
            });
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

        public static void RemoveFinishedCB(AppDbContext context, int villageId)
        {
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

        private static PlanTask GetFirstResTask(IPlanManager planManager, int villageId)
        {
            var tasks = planManager.GetList(villageId);
            var task = tasks.FirstOrDefault(x => x.Type == PlanTypeEnums.ResFields || x.Building.IsResourceField());
            return task;
        }

        private static PlanTask GetFirstInfrastructureTask(IPlanManager planManager, int villageId)
        {
            var tasks = planManager.GetList(villageId);
            var task = tasks.FirstOrDefault(x => x.Type == PlanTypeEnums.General && !x.Building.IsResourceField());
            return task;
        }

        private static PlanTask GetFirstTask(IPlanManager planManager, int villageId)
        {
            var tasks = planManager.GetList(villageId);
            var task = tasks.FirstOrDefault();
            return task;
        }

        public static DateTime GetTimeWhenEnough(this VillageProduction production, long[] resRequired)
        {
            var productionArr = new long[] { production.Wood, production.Clay, production.Iron, production.Crop };

            var now = DateTime.Now;
            var ret = now.AddMinutes(2);

            for (int i = 0; i < 4; i++)
            {
                DateTime toWaitForThisRes = DateTime.MinValue;
                if (resRequired[i] > 0)
                {
                    // In case of negative crop, we will never have enough crop
                    if (productionArr[i] <= 0) return DateTime.MaxValue;

                    float hoursToWait = resRequired[i] / (float)productionArr[i];
                    float secToWait = hoursToWait * 3600;
                    toWaitForThisRes = now.AddSeconds(secToWait);
                }

                if (ret < toWaitForThisRes) ret = toWaitForThisRes;
            }
            return ret;
        }
    }
}