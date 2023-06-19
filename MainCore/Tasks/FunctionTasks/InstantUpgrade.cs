using FluentResults;
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
    public sealed class InstantUpgrade : VillageBotTask
    {
        private readonly ICompleteNowHelper _completeNowHelper;
        private readonly ITaskManager _taskManager;

        public InstantUpgrade(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _completeNowHelper = Locator.Current.GetService<ICompleteNowHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var result = _completeNowHelper.Execute(AccountId, VillageId);
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
                _taskManager.ReOrder(AccountId);
            }
        }
    }
}