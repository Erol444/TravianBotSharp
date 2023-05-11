using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class RallypointHelper : IRallypointHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly IGeneralHelper _generalHelper;
        protected readonly IUpdateHelper _updateHelper;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public RallypointHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, IUpdateHelper updateHelper)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
            _updateHelper = updateHelper;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
            _generalHelper.Load(villageId, accountId, cancellationToken);
            _updateHelper.Load(villageId, accountId, cancellationToken);
        }

        public Result EnterFarmList()
        {
            _result = _generalHelper.SwitchVillage();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = _generalHelper.ToDorf2();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = ToRallypoint();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = _generalHelper.SwitchTab(4);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = _updateHelper.UpdateFarmList();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            return Result.Ok();
        }

        public Result StartFarmList()
        {
            if (!_generalHelper.IsPageValid()) return Result.Fail(Stop.Announcement);

            _result = EnterFarmList();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == _accountId);
            foreach (var farm in farms)
            {
                if (_token.IsCancellationRequested) return Result.Fail(new Cancel());
                var isActive = context.FarmsSettings.Find(farm.Id).IsActive;
                if (!isActive) continue;

                var result = ClickStartFarm(farm.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        protected Result ToRallypoint()
        {
            using var context = _contextFactory.CreateDbContext();
            var rallypoint = context.VillagesBuildings.Where(x => x.VillageId == _villageId)
                                                      .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
            if (rallypoint is null)
            {
                return Result.Fail(new Skip("Rallypoint is missing"));
            }

            _result = _generalHelper.ToBuilding(rallypoint.Id);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        protected abstract Result ClickStartFarm(int farmId);
    }
}