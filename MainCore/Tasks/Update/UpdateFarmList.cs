using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateFarmList : AccountBotTask
    {
        private readonly IUpdateHelper _updateHelper;
        private readonly ICheckHelper _checkHelper;
        private int _villageHasRallyPoint;

        public UpdateFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _checkHelper = Locator.Current.GetService<ICheckHelper>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                CheckRayllyPoint,
                GotoFarmListPage,
                Update,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result CheckRayllyPoint()
        {
            _villageHasRallyPoint = GetVillageHasRallyPoint();
            if (_villageHasRallyPoint == -1)
            {
                return Result.Fail(new Skip("There is no rallypoint in your villages");
            }
            return Result.Ok();
        }

        private Result GotoFarmListPage()
        {
            if (_checkHelper.IsFarmListPage(AccountId)) return Result.Ok();

            {
                var taskUpdate = new UpdateDorf2(_villageHasRallyPoint, AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = _navigateHelper.GoToBuilding(AccountId, 39);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _navigateHelper.SwitchTab(AccountId, 4);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        private Result Update()
        {
            var result = _updateHelper.UpdateFarmList(AccountId);
            _eventManager.OnFarmListUpdate(AccountId);
            return result;
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
    }
}