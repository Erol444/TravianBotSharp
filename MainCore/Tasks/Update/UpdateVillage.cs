using MainCore.Enums;
using MainCore.Helper;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Sim;
using System;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : VillageBotTask
    {
        public UpdateVillage(int villageId, int accountId) : base(villageId, accountId, "Update village")
        {
        }

        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            IsFail = true;
            Navigate();
            if (IsUpdateAccountInfoFail()) return;
            UpdateVillageInfo();
            IsFail = false;
        }

        private void Navigate()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
        }

        private bool IsUpdateAccountInfoFail()
        {
            var updateTask = new UpdateInfo(AccountId);
            updateTask.CopyFrom(this);
            updateTask.Execute();
            return updateTask.IsFail;
        }

        private void UpdateVillageInfo()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                UpdateHelper.UpdateCurrentlyBuilding(context, _chromeBrowser, VillageId);
                InstantUpgrade(context);
                _eventManager.OnVillageCurrentUpdate(VillageId);
            }
            if (currentUrl.Contains("dorf1"))
            {
                UpdateHelper.UpdateDorf1(context, _chromeBrowser, VillageId);
                _eventManager.OnVillageBuildsUpdate(VillageId);

                UpdateHelper.UpdateProduction(context, _chromeBrowser, VillageId);
            }
            else if (currentUrl.Contains("dorf2"))
            {
                UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, VillageId);
                AutoImproveTroop(context);
                _eventManager.OnVillageBuildsUpdate(VillageId);
            }

            UpdateHelper.UpdateResource(context, _chromeBrowser, VillageId);
            AutoNPC(context);
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
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            if (tribe == TribeEnums.Romans)
            {
                if (currentlyBuildings.Count(x => x.Level != -1) < (info.HasPlusAccount ? 3 : 2)) return;
            }
            else
            {
                if (currentlyBuildings.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
            }
#elif TTWARS
            if (currentlyBuildings.Count(x => x.Level != -1) < (info.HasPlusAccount ? 2 : 1)) return;
#else

#error You forgot to define Travian version here

#endif
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
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            if (info.Gold < 3) return;
#elif TTWARS
            if (info.Gold < 5) return;

#else

#error You forgot to define Travian version here

#endif

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
    }
}