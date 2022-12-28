using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class RefreshVillage : VillageBotTask
    {
        private readonly int _mode;

        /// <summary>
        ///
        /// </summary>
        /// <param name="villageId"></param>
        /// <param name="accountId"></param>
        /// <param name="mode"> 0 = auto detect 1 = dorf 1 2 = dorf2 3 = both dorf</param>
        /// <param name="cancellationToken"></param>
        public RefreshVillage(int villageId, int accountId, int mode = 0, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _mode = mode;
        }

        public override Result Execute()
        {
            BotTask taskUpdate;
            switch (_mode)
            {
                case 0:
                    taskUpdate = IsNeedDorf2() ? new UpdateBothDorf(VillageId, AccountId, CancellationToken) : new UpdateDorf1(VillageId, AccountId, CancellationToken);
                    break;

                case 1:
                    taskUpdate = new UpdateDorf1(VillageId, AccountId, CancellationToken);
                    break;

                case 2:
                    taskUpdate = new UpdateDorf2(VillageId, AccountId, CancellationToken);
                    break;

                case 3:
                    taskUpdate = new UpdateBothDorf(VillageId, AccountId, CancellationToken);
                    break;

                default:
                    return Result.Ok();
            }

            var result = taskUpdate.Execute();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            ApplyAutoTask();
            NextExecute();
            return Result.Ok();
        }

        private bool IsNeedDorf2()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            return setting.IsUpgradeTroop;
        }

        private void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            var delay = Random.Shared.Next(setting.AutoRefreshTimeMin, setting.AutoRefreshTimeMax);
            ExecuteAt = DateTime.Now.AddMinutes(delay);
        }

        private void ApplyAutoTask()
        {
            using var context = _contextFactory.CreateDbContext();

            InstantUpgrade(context);
            AutoNPC(context);

            if (_mode == 2 || IsNeedDorf2()) AutoImproveTroop(context);
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