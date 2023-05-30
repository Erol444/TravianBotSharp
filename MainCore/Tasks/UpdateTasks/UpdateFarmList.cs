using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.UpdateTasks
{
    public sealed class UpdateFarmList : AccountBotTask
    {
        private readonly IRallypointHelper _rallypointHelper;
        private readonly ICheckHelper _checkHelper;

        public UpdateFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _rallypointHelper = Locator.Current.GetService<IRallypointHelper>();
            _checkHelper = Locator.Current.GetService<ICheckHelper>();
        }

        public override Result Execute()
        {
            var resultVillage = GetVillageHasRallyPoint();
            if (resultVillage.IsFailed) return Result.Fail(resultVillage.Errors).WithError(new Trace(Trace.TraceMessage()));

            var result = _rallypointHelper.EnterFarmListPage(AccountId, resultVillage.Value);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result<int> GetVillageHasRallyPoint()
        {
            using var context = _contextFactory.CreateDbContext();

            // check current village
            var result = _checkHelper.GetCurrentVillageId(AccountId);
            if (result.IsFailed) return Result.Fail(result.Errors).WithError(new Trace(Trace.TraceMessage()));
            var currentVillage = result.Value;

            var rallyPointCurrentVillage = context.VillagesBuildings
               .Where(x => x.VillageId == currentVillage)
               .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
            if (rallyPointCurrentVillage is not null) return currentVillage;

            // check other village
            var villages = context.Villages.Where(x => x.AccountId == AccountId);

            foreach (var village in villages)
            {
                if (village.Id == currentVillage) continue;

                var rallyPoint = context.VillagesBuildings
                    .Where(x => x.VillageId == village.Id)
                    .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (rallyPoint is null) continue;
                return village.Id;
            }
            return Result.Fail(new Skip("No village has rally point"));
        }
    }
}