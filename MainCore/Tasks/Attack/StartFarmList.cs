using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Update;
using Splat;
using System;
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

            if (!IsFarmExist())
            {
                _logManager.Warning(AccountId, $"Farm {FarmId} is missing. Remove this farm from queue");
                return Result.Ok();
            }

            if (IsFarmDeactive())
            {
                _logManager.Warning(AccountId, $"Farm {FarmId} is deactive. Remove this farm from queue");
                return Result.Ok();
            }
            {
                var result = _clickHelper.ClickStartFarm(AccountId, FarmId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.FarmsSettings.Find(FarmId);
                var time = Random.Shared.Next(setting.IntervalMin, setting.IntervalMax);
                ExecuteAt = DateTime.Now.AddSeconds(time);
            }
            return Result.Ok();
        }

        private bool IsFarmExist()
        {
            using var context = _contextFactory.CreateDbContext();
            var farm = context.Farms.Find(FarmId);
            return farm is not null;
        }

        private bool IsFarmDeactive()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.FarmsSettings.Find(FarmId);
            if (!setting.IsActive) return true;
            return false;
        }
    }
}