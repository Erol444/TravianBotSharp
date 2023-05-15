using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class StartFarmList : AccountBotTask
    {
        private readonly IRallypointHelper _rallypointHelper;
        private readonly ICheckHelper _checkHelper;

        public StartFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _rallypointHelper = Locator.Current.GetService<IRallypointHelper>();
            _checkHelper = Locator.Current.GetService<ICheckHelper>(); ;
        }

        public override Result Execute()
        {
            _checkHelper.Load(-1, AccountId, CancellationToken);
            _rallypointHelper.Load(_checkHelper.GetCurrentVillageId(), AccountId, CancellationToken);

            var result = _rallypointHelper.StartFarmList();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            SetNextExecute();
            return Result.Ok();
        }

        private void SetNextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            var time = Random.Shared.Next(setting.FarmIntervalMin, setting.FarmIntervalMax);
            ExecuteAt = DateTime.Now.AddSeconds(time);
        }
    }
}