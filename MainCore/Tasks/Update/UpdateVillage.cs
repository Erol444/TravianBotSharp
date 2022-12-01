using FluentResults;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Sim;
using Microsoft.EntityFrameworkCore;
using Splat;
using System;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : VillageBotTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private IChromeBrowser _chromeBrowser;

        private readonly INavigateHelper _navigateHelper;
        private readonly IUpdateHelper _updateHelper;
        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;

        public UpdateVillage(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, INavigateHelper navigateHelper, IUpdateHelper updateHelper, IEventManager eventManager, ITaskManager taskManager)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _chromeBrowser = chromeBrowser;
            _navigateHelper = navigateHelper;
            _updateHelper = updateHelper;
            _eventManager = eventManager;
            _taskManager = taskManager;
        }

        public override Result Execute()
        {
            _chromeBrowser = _chromeManager.Get(AccountId);
            {
                using var context = _contextFactory.CreateDbContext();
                var village = context.Villages.Find(VillageId);
                if (village is null) Name = $"Update village {VillageId}";
                else Name = $"Update village {village.Name}";
            }
            {
                var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                if (result.IsFailed) return result.WithError("from Update village");
            }
            {
                var result = UpdateAccountInfo();
                if (result.IsFailed) return result.WithError("from Update village");
            }
            {
                var result = UpdateVillageInfo();
                if (result.IsFailed) return result.WithError("from Update village");
            }
            return Result.Ok();
        }

        private Result UpdateAccountInfo()
        {
            var updateTask = Locator.Current.GetService<UpdateInfo>();
            return updateTask.Execute();
        }

        private Result UpdateVillageInfo()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                {
                    var result = _updateHelper.UpdateCurrentlyBuilding(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError("from Update village info");
                }
                InstantUpgrade();
                _eventManager.OnVillageCurrentUpdate(VillageId);
            }
            if (currentUrl.Contains("dorf1"))
            {
                {
                    var result = _updateHelper.UpdateDorf1(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError("from Update village info");
                }
                _eventManager.OnVillageBuildsUpdate(VillageId);
                {
                    var result = _updateHelper.UpdateProduction(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError("from Update village info");
                }
            }
            else if (currentUrl.Contains("dorf2"))
            {
                {
                    var result = _updateHelper.UpdateDorf2(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError("from Update village info");
                }
                AutoImproveTroop();
                _eventManager.OnVillageBuildsUpdate(VillageId);
            }

            {
                var result = _updateHelper.UpdateResource(AccountId, VillageId);
                if (result.IsFailed) return result.WithError("from Update village info");
            }
            AutoNPC();
            return Result.Ok();
        }

        private void InstantUpgrade()
        {
            using var context = _contextFactory.CreateDbContext();
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

        private void AutoNPC()
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<NPCTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            using var context = _contextFactory.CreateDbContext();
            var info = context.AccountsInfo.Find(AccountId);
            if (VersionDetector.IsTravianOfficial())
            {
                if (info.Gold < 3) return;
            }
            else if (VersionDetector.IsTTWars())
            {
                if (info.Gold < 5) return;
            }

            var setting = context.VillagesSettings.Find(VillageId);
            if (!setting.IsAutoNPC) return;
            if (setting.AutoNPCPercent == 0) return;
            if (setting.AutoNPCWarehousePercent == 0) return;

            var resource = context.VillagesResources.Find(VillageId);
            var ratioGranary = resource.Crop * 100.0f / resource.Granary;
            var maxResource = Math.Max(resource.Wood, Math.Max(resource.Clay, resource.Iron));
            var ratioWarehouse = maxResource * 100.0f / resource.Warehouse;
            if (ratioGranary < setting.AutoNPCPercent && ratioWarehouse < setting.AutoNPCWarehousePercent) return;

            _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
        }

        private void AutoImproveTroop()
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<ImproveTroopsTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            using var context = _contextFactory.CreateDbContext();
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
    }
}