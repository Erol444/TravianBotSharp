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

        private readonly ITaskManager _taskManager;

        public RefreshVillage(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            Result result;
            if (IsNeedDorf2())
            {
                result = _generalHelper.ToDorf2(AccountId, VillageId, switchVillage: true);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            result = _generalHelper.ToDorf1(AccountId, VillageId, switchVillage: true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            NextExecute();
            return Result.Ok();
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
                _taskManager.Add<ImproveTroopsTask>(AccountId, VillageId);
            }
        }
    }
}