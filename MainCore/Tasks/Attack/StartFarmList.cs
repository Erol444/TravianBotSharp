using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Update;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Attack
{
    public class StartFarmList : AccountBotTask
    {
        private readonly IClickHelper _clickHelper;

        public StartFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        public override Result Execute()
        {
            {
                var updateTask = new UpdateFarmList(AccountId);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = ClickStartFarm();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.AccountsSettings.Find(AccountId);
                var time = Random.Shared.Next(setting.FarmIntervalMin, setting.FarmIntervalMax);
                ExecuteAt = DateTime.Now.AddSeconds(time);
            }
            return Result.Ok();
        }

        private Result ClickStartFarm()
        {
            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == AccountId).ToList();
            foreach (var farm in farms)
            {
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                var isActive = context.FarmsSettings.Find(farm.Id).IsActive;
                if (!isActive) continue;

                var result = _clickHelper.ClickStartFarm(AccountId, farm.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}