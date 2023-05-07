using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class TrainTroopsTask : VillageBotTask
    {
        private readonly List<BuildingEnums> _buildings = new();

        private readonly ITrainTroopHelper _trainTroopHelper;

        public TrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _trainTroopHelper = Locator.Current.GetService<ITrainTroopHelper>();
        }

        public override Result Execute()
        {
            _trainTroopHelper.Load(VillageId, AccountId, CancellationToken);
            CheckBuilding();

            var result = SwitchVillage();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            foreach (var building in _buildings)
            {
                result = _trainTroopHelper.Execute(building);
                if (result.IsFailed)
                {
                    if (result.HasError<NoResource>())
                    {
                        _logManager.Warning(AccountId, result.Reasons[0].Message);
                        break;
                    }
                    return result.WithError(new Trace(Trace.TraceMessage()));
                }
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            NextExecute();
            return Result.Ok();
        }

        private void CheckBuilding()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);

            _buildings.Clear();
            if (setting.BarrackTroop != 0)
            {
                _buildings.Add(BuildingEnums.Barracks);
                if (setting.IsGreatBarrack)
                {
                    _buildings.Add(BuildingEnums.GreatBarracks);
                }
            }

            if (setting.StableTroop != 0)
            {
                _buildings.Add(BuildingEnums.Stable);
                if (setting.IsGreatBarrack)
                {
                    _buildings.Add(BuildingEnums.GreatStable);
                }
            }

            if (setting.WorkshopTroop != 0)
            {
                _buildings.Add(BuildingEnums.Workshop);
            }
        }

        private Result SwitchVillage()
        {
            return _navigateHelper.SwitchVillage(AccountId, VillageId);
        }

        private void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);

            ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(setting.TroopTimeMin, setting.TroopTimeMax));
        }
    }
}