using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Update;
using Splat;
using System;

namespace MainCore.Tasks.Attack
{
    public class StartFarmList : AccountBotTask
    {
        private readonly INavigateHelper _navigateHelper;
        private readonly IClickHelper _clickHelper;

        public StartFarmList(int accountId, int farmId) : base(accountId)
        {
            _farmId = farmId;
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        private readonly int _farmId;
        public int FarmId => _farmId;

        public override string GetName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                using var context = _contextFactory.CreateDbContext();
                var farm = context.Farms.Find(FarmId);
                if (farm is not null)
                {
                    _name = $"Start list farm [{farm.Name}]";
                }
                else
                {
                    _name = $"Start list farm [unknow]";
                }
            }
            return _name;
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

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.FarmsSettings.Find(FarmId);
                var time = Random.Shared.Next(setting.IntervalMin, setting.IntervalMax);
                ExecuteAt = DateTime.Now.AddSeconds(time);
            }
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