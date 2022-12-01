using FluentResults;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Update;
using Microsoft.EntityFrameworkCore;
using Splat;
using System;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace MainCore.Tasks.Attack
{
    public class StartFarmList : AccountBotTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogManager _logManager;
        private IChromeBrowser _chromeBrowser;

        private readonly IClickHelper _clickHelper;

        public StartFarmList(ILogManager logManager, IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IClickHelper clickHelper)
        {
            _logManager = logManager;
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _clickHelper = clickHelper;
        }

        private int _farmId;
        public int FarmId => _farmId;

        public void SetFarmId(int farmId)
        {
            _farmId = farmId;
        }

        public override Result Execute()
        {
            _chromeBrowser = _chromeManager.Get(AccountId);
            {
                var result = UpdateFarmlist();
                if (result.IsFailed) return result.WithError("from update farm list");
            }

            if (!IsFarmExist())
            {
                _logManager.Warning(AccountId, $"Farm {FarmId} is missing. Remove this farm from queue");
                return Result.Ok();
            }
            if (IsFarmDeactive())
            {
                using var context = _contextFactory.CreateDbContext();
                var farm = context.Farms.Find(FarmId);
                _logManager.Warning(AccountId, $"Farm {farm.Name} is deactive. Remove this farm from queue");
                return Result.Ok();
            }
            {
                var result = _clickHelper.ClickStartFarm(AccountId, FarmId);
                if (result.IsFailed) return result.WithError("from click start farm");
            }

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.FarmsSettings.Find(FarmId);
                var time = Random.Shared.Next(setting.IntervalMin, setting.IntervalMax);
                var farm = context.Farms.Find(FarmId);
                ExecuteAt = DateTime.Now.AddSeconds(time);
                _logManager.Information(AccountId, $"Farmlist {farm.Name} was sent.");
            }
            return Result.Ok();
        }

        private Result UpdateFarmlist()
        {
            var updateTask = Locator.Current.GetService<UpdateFarmList>();
            updateTask.SetAccountId(AccountId);
            return updateTask.Execute();
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