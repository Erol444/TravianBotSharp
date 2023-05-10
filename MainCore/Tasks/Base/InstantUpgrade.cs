using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class InstantUpgrade : VillageBotTask
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly ICompleteNowHelper _completeNowHelper;

        public InstantUpgrade(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
            _completeNowHelper = Locator.Current.GetService<ICompleteNowHelper>();
        }

        public override Result Execute()
        {
            _generalHelper.Load(VillageId, AccountId, CancellationToken);
            Result result;

            result = _generalHelper.SwitchVillage();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            result = _generalHelper.ToDorf();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            result = _completeNowHelper.Execute();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            TriggerTask();

            result = _generalHelper.ToDorf(true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

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

        protected Result Update()
        {
            var updateTask = new UpdateVillage(VillageId, AccountId, CancellationToken);
            var result = updateTask.Execute();
            return result;
        }
    }
}