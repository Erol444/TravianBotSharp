using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class RallypointHelper : IRallypointHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly IGeneralHelper _generalHelper;
        protected readonly IUpdateHelper _updateHelper;

        public RallypointHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, IUpdateHelper updateHelper)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
            _updateHelper = updateHelper;
        }

        public abstract Result ClickStartFarm(int accountId, int farmId);

        public Result EnterFarmListPage(int accountId, int villageId)
        {
            var result = _generalHelper.SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.ToDorf2(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ToRallypoint(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.SwitchTab(accountId, 4);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _updateHelper.UpdateFarmList();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result StartFarmList(int accountId, int villageId)
        {
            var result = EnterFarmListPage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            using var context = _contextFactory.CreateDbContext();
            var farms = context.Farms.Where(x => x.AccountId == accountId);
            foreach (var farm in farms)
            {
                var isActive = context.FarmsSettings.Find(farm.Id).IsActive;
                if (!isActive) continue;

                result = ClickStartFarm(accountId, farm.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result ToRallypoint(int accountId, int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var rallypoint = context.VillagesBuildings.Where(x => x.VillageId == villageId)
                                                      .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
            if (rallypoint is null)
            {
                return Result.Fail(new Skip("Rallypoint is missing"));
            }

            var result = _generalHelper.ToBuilding(accountId, rallypoint.Id);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}