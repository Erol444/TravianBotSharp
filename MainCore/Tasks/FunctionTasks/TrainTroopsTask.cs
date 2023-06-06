using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class TrainTroopsTask : VillageBotTask
    {
        private readonly List<BuildingEnums> _buildings = new();

        private readonly ITrainTroopHelper _trainTroopHelper;
        private readonly IGeneralHelper _generalHelper;

        private readonly Services.Interface.ILogManager _logManager;

        public TrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _trainTroopHelper = Locator.Current.GetService<ITrainTroopHelper>();
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
            _logManager = Locator.Current.GetService<Services.Interface.ILogManager>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            NextExecute();

            CheckBuilding();

            var result = _generalHelper.SwitchVillage(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            foreach (var building in _buildings)
            {
                result = _trainTroopHelper.Execute(AccountId, VillageId, building);
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

        private void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);

            ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(setting.TroopTimeMin, setting.TroopTimeMax));
        }
    }
}