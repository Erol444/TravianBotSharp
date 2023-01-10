using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateFarmList : AccountBotTask
    {
        private readonly IUpdateHelper _updateHelper;
        private readonly ICheckHelper _checkHelper;

        public UpdateFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _checkHelper = Locator.Current.GetService<ICheckHelper>();
        }

        public override Result Execute()
        {
            var village = GetVillageHasRallyPoint();
            if (village == -1)
            {
                _logManager.Warning(AccountId, "There is no rallypoint in your villages", this);
                return Result.Ok();
            }
            {
                var result = GotoFarmListPage(village);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _updateHelper.UpdateFarmList(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                _eventManager.OnFarmListUpdate(AccountId);
            }
            return Result.Ok();
        }

        private int GetVillageHasRallyPoint()
        {
            using var context = _contextFactory.CreateDbContext();

            var currentVillage = _checkHelper.GetCurrentVillageId(AccountId);
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

        private Result GotoFarmListPage(int village)
        {
            if (_checkHelper.IsFarmListPage(AccountId)) return Result.Ok();

            {
                var taskUpdate = new UpdateDorf2(village, AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            _navigateHelper.GoToBuilding(AccountId, 39);
            _navigateHelper.SwitchTab(AccountId, 4);

            return Result.Ok();
        }
    }
}