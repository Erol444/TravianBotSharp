using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public sealed class InstantUpgrade : VillageBotTask
    {
        private readonly ICompleteNowHelper _completeNowHelper;

        public InstantUpgrade(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _completeNowHelper = Locator.Current.GetService<ICompleteNowHelper>();
        }

        public override Result Execute()
        {
            _completeNowHelper.Load(VillageId, AccountId, CancellationToken);

            var result = _completeNowHelper.Execute();
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            TriggerTask();

            return Result.Ok();
        }

        private void TriggerTask()
        {
            var tasks = _taskManager.GetList(AccountId);
            //var improveTroopTask = tasks.OfType<ImproveTroopsTask>().FirstOrDefault(x => x.VillageId == VillageId);
            //if (improveTroopTask is not null)
            //{
            //    improveTroopTask.ExecuteAt = DateTime.Now;
            //    _taskManager.Update(AccountId);
            //}
            var upgradeTask = tasks.OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == VillageId);
            if (upgradeTask is not null)
            {
                upgradeTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }
        }
    }
}