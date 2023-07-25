using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class TrainTroopsTask : VillageBotTask
    {
        private readonly List<BuildingEnums> _buildings = new();

        private readonly ITrainTroopHelper _trainTroopHelper;
        private readonly IGeneralHelper _generalHelper;

        private readonly ILogHelper _logHelper;

        public TrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _trainTroopHelper = Locator.Current.GetService<ITrainTroopHelper>();
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
            _logHelper = Locator.Current.GetService<ILogHelper>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            _buildings.Clear();

            NextExecute();

            CheckBuilding();

            _logHelper.Information(AccountId, $"Is going to train in {string.Join(", ", _buildings.Select(x => x.ToString()))}");

            var result = _generalHelper.ToDorf2(AccountId, VillageId, switchVillage: true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            foreach (var building in _buildings)
            {
                _logHelper.Information(AccountId, $"Start train in {building}");
                result = _trainTroopHelper.Execute(AccountId, VillageId, building);
                if (result.IsFailed)
                {
                    if (result.HasError<NoResource>())
                    {
                        _logHelper.Warning(AccountId, result.Reasons[0].Message);
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
                if (setting.IsGreatStable)
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