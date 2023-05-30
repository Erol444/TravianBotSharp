using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class RefreshVillage : VillageBotTask
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly INPCHelper _npcHelper;

        private readonly ITaskManager _taskManager;

        public RefreshVillage(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
            _npcHelper = Locator.Current.GetService<INPCHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public override Result Execute()
        {
            Result result;
            if (IsNeedDorf2())
            {
                result = _generalHelper.ToDorf2(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            result = _generalHelper.ToDorf1(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            ApplyAutoTask();
            NextExecute();
            return Result.Ok();
        }

        private void ApplyAutoTask()
        {
            InstantUpgrade();
            AutoNPC();

            //if (IsNeedDorf2()) AutoImproveTroop();
        }

        private void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);

            if (!setting.IsAutoRefresh) return;

            var delay = Random.Shared.Next(setting.AutoRefreshTimeMin, setting.AutoRefreshTimeMax);
            ExecuteAt = DateTime.Now.AddMinutes(delay);
        }

        private bool IsNeedDorf2()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            return setting.IsUpgradeTroop;
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

            if (!_npcHelper.IsEnoughGold(AccountId)) return;

            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);

            var resource = context.VillagesResources.Find(VillageId);
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
        }

        private void AutoImproveTroop()
        {
            using var context = _contextFactory.CreateDbContext();

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