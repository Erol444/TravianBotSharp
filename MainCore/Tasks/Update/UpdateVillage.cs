using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Sim;
using Splat;
using System;
using System.Linq;
using System.Diagnostics;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : VillageBotTask
    {
        private readonly INavigateHelper _navigateHelper;
        private readonly IUpdateHelper _updateHelper;

        public UpdateVillage(int villageId, int accountId) : base(villageId, accountId)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var updateTask = new UpdateInfo(AccountId);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = UpdateVillageInfo();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        private Result UpdateVillageInfo()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                var result = _updateHelper.UpdateCurrentlyBuilding(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                InstantUpgrade(context);
                _eventManager.OnVillageCurrentUpdate(VillageId);
            }
            if (currentUrl.Contains("dorf1"))
            {
                {
                    var result = _updateHelper.UpdateDorf1(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                _eventManager.OnVillageBuildsUpdate(VillageId);

                {
                    var result = _updateHelper.UpdateProduction(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else if (currentUrl.Contains("dorf2"))
            {
                {
                    var result = _updateHelper.UpdateDorf2(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                AutoImproveTroop(context);
                _eventManager.OnVillageBuildsUpdate(VillageId);
            }

            {
                var result = _updateHelper.UpdateResource(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            AutoNPC(context);
<<<<<<< HEAD

            AutoSendResourcesOut(context);
            AutoSendResourcesIn(context);
=======
            return Result.Ok();
>>>>>>> release/2.4.0
        }

        private void InstantUpgrade(AppDbContext context)
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<InstantUpgrade>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsInstantComplete) return;
            var info = context.AccountsInfo.Find(AccountId);
            if (info.Gold < 2) return;
            var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Level != -1).ToList();

            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            if (tribe == TribeEnums.Romans)
            {
                if (currentlyBuildings.Count(x => x.Level != -1) < (info.HasPlusAccount ? 3 : 2)) return;
            }
            else
            {
                if (currentlyBuildings.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
            }
            var notInstantBuildings = currentlyBuildings.Where(x => x.Type.IsNotAdsUpgrade());
            foreach (var building in notInstantBuildings)
            {
                currentlyBuildings.Remove(building);
            }
            if (!currentlyBuildings.Any()) return;

            if (currentlyBuildings.Max(x => x.CompleteTime) < DateTime.Now.AddMinutes(setting.InstantCompleteTime)) return;

            _taskManager.Add(AccountId, new InstantUpgrade(VillageId, AccountId));
        }


        private void AutoNPC(AppDbContext context)
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<NPCTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            var info = context.AccountsInfo.Find(AccountId);

            var goldNeed = 0;
            if (VersionDetector.IsTravianOfficial())
            {
                goldNeed = 3;
            }
            else if (VersionDetector.IsTTWars())
            {
                goldNeed = 5;
            }
            if (info.Gold < goldNeed) return;

            var setting = context.VillagesSettings.Find(VillageId);

            var resource = context.VillagesResources.Find(VillageId);
<<<<<<< HEAD

            var ratioGranary = resource.Crop * 100.0f / resource.Granary;
            var maxResource = Math.Max(resource.Wood, Math.Max(resource.Clay, resource.Iron));
            var ratioWarehouse = maxResource * 100.0f / resource.Warehouse;
            if (ratioGranary < setting.AutoNPCPercent && ratioWarehouse < setting.AutoNPCWarehousePercent) return;

            _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
=======
            if (setting.IsAutoNPC && setting.AutoNPCPercent != 0)
            {
                var ratio = resource.Crop * 100.0f / resource.Granary;
                if (ratio < setting.AutoNPCPercent) return;
                _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
            }
            if (setting.IsAutoNPCWarehouse && setting.AutoNPCWarehousePercent != 0)
            {
                var maxResource = Math.Max(resource.Wood, Math.Max(resource.Clay, resource.Iron));
                var ratio = maxResource * 100.0f / resource.Warehouse;
                if (ratio < setting.AutoNPCWarehousePercent) return;
                _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
            }
>>>>>>> release/2.4.0
        }

        private void AutoImproveTroop(AppDbContext context)
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<ImproveTroopsTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsUpgradeTroop) return;
            var troopsUpgrade = setting.GetTroopUpgrade();
            if (!troopsUpgrade.Any(x => x)) return;

            var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId).ToList();
            var smithy = buildings.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
            if (smithy is null) return;
            var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId).ToArray();

            for (int i = 0; i < troops.Length; i++)
            {
                if (!troopsUpgrade[i]) continue;
                if (troops[i].Level == -1) continue;
                if (troops[i].Level >= smithy.Level) continue;
                _taskManager.Add(AccountId, new ImproveTroopsTask(VillageId, AccountId));
            }
        }

        private void AutoSendResourcesOut(AppDbContext context)
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<SendResourcesOutTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            var setting = context.VillagesMarket.Find(VillageId);
            if (!setting.IsSendExcessResources) return;

            _taskManager.Add(AccountId, new SendResourcesOutTask(VillageId, AccountId));
        }

        private void AutoSendResourcesIn(AppDbContext context)
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<SendResourcesInTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            var setting = context.VillagesMarket.Find(VillageId);
            if (!setting.IsGetMissingResources) return;

            _taskManager.Add(AccountId, new SendResourcesInTask(VillageId, AccountId));
        }
    }
}