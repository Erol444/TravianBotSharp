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
            _checkHelper.Load(-1, AccountId, CancellationToken);
            _rallypointHelper.Load(GetVillageHasRallyPoint(), AccountId, CancellationToken);

            var result = _rallypointHelper.EnterFarmListPage();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private int GetVillageHasRallyPoint()
        {
            using var context = _contextFactory.CreateDbContext();

            var currentVillage = _checkHelper.GetCurrentVillageId();
            if (currentVillage != -1)
            {
                var building = context.VillagesBuildings
                   .Where(x => x.VillageId == currentVillage)
                   .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (building is not null) return currentVillage;
            }

            var villages = context.Villages.Where(x => x.AccountId == AccountId);

            foreach (var village in villages)
            {
                var building = context.VillagesBuildings
                    .Where(x => x.VillageId == village.Id)
                    .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (building is null) continue;
                return village.Id;
            }
            return -1;
        }
    }
}