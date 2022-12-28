using FluentResults;
using MainCore.Errors;
using MainCore.Tasks.Update;
using System;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class RefreshVillage : VillageBotTask
    {
        public RefreshVillage(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            BotTask taskUpdate;
            if (IsNeedDorf2())
            {
                taskUpdate = new UpdateBothDorf(VillageId, AccountId, CancellationToken);
            }
            else
            {
                taskUpdate = new UpdateDorf1(VillageId, AccountId, CancellationToken);
            }
            var result = taskUpdate.Execute();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

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
    }
}