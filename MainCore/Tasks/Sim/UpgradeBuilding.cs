using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainCore.Tasks.Sim
{
    public class UpgradeBuilding : BotTask
    {
        public UpgradeBuilding(int villageId, int accountId) : base(accountId)
        {
            _villageId = villageId;
        }

        public override string Name => $"Upgrade building village {VillageId}";

        private readonly int _villageId;
        public int VillageId => _villageId;

        public override async Task Execute()
        {
            using var context = ContextFactory.CreateDbContext();

            do
            {
                var buildingTask = UpgradeBuildingHelper.NextBuildingTask(context, PlanManager, LogManager, AccountId, VillageId);
                if (buildingTask is null)
                {
                    var tasks = PlanManager.GetList(VillageId);
                    if (tasks.Count == 0)
                    {
                        LogManager.Information(AccountId, "Queue is empty.");
                        return;
                    }

                    NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
                    NavigateHelper.GoRandomDorf(ChromeBrowser);
                    UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, VillageId);

                    var firstComplete = context.VillagesCurrentlyBuildings.Find(VillageId, 0);
                    if (firstComplete.CompleteTime == DateTime.MaxValue)
                    {
                        continue;
                    }

                    ExecuteAt = firstComplete.CompleteTime.AddSeconds(10);
                    LogManager.Information(AccountId, $"Next building will be contructed after {firstComplete.Type} - level {firstComplete.Level} complete. ({ExecuteAt})");
                    break;
                }

                if (buildingTask.Type == PlanTypeEnums.ResFields)
                {
                    var task = UpgradeBuildingHelper.ExtractResField(context, VillageId, buildingTask);
                    if (task is null)
                    {
                        PlanManager.Remove(VillageId, task);
                        continue;
                    }
                    else
                    {
                        PlanManager.Insert(VillageId, 0, task);
                        continue;
                    }
                }

                NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
                UpdateHelper.UpdateResource(context, ChromeBrowser, VillageId);

                if (context.VillagesResources.Find(VillageId).FreeCrop <= 5 && buildingTask.Building != BuildingEnums.Cropland)
                {
                    var cropland = context.VillagesBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Type == BuildingEnums.Cropland).OrderBy(x => x.Level).FirstOrDefault();
                    var task = new PlanTask()
                    {
                        Type = PlanTypeEnums.General,
                        Level = cropland.Level + 1,
                        Building = BuildingEnums.Cropland,
                        Location = cropland.Id,
                    };
                    PlanManager.Insert(VillageId, 0, task);
                    continue;
                }
            }
            while (true);
        }

        private bool ExtractResField(AppDbContext context)
        {
            LogManager.Information(AccountId, "This is task auto upgrade res field. Choose what res fields will upgrade");
            if (task is null)
            {
                Retry("There is not buiding in progress. Try get another building");
                return false;
            }
            acc.Logger.Information($"Added {task.Building} - Level {task.Level} to queue");
            return false;
        }
    }
}