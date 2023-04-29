using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class TrainTroopsTask : VillageBotTask
    {
        private readonly BuildingEnums _trainingBuilding;
        protected BuildingEnums TrainingBuilding => _trainingBuilding;

        public TrainTroopsTask(int villageId, int accountId, BuildingEnums trainingBuilding, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _trainingBuilding = trainingBuilding;
        }

        public override Result Execute()
        {
            {
                var result = SwitchVillage();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            {
                var result = UpdateDorf2();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            {
                var result = HasBuilding();
                if (!result)
                {
                    DisableSetting();
                    _logManager.Information(AccountId, "There is no building for training troops");
                    return Result.Ok();
                }
            }
            {
                var result = EnterBuilding(false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result SwitchVillage()
        {
            return _navigateHelper.SwitchVillage(AccountId, VillageId);
        }

        private Result UpdateDorf2()
        {
            return _navigateHelper.ToDorf2(AccountId);
        }

        private bool HasBuilding(bool great = false)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId);

            var buildingType = great ? TrainingBuilding.GetGreatVersion() : TrainingBuilding;

            return buildings.Any(x => x.Type == TrainingBuilding && x.Level > 0);
        }

        private void DisableSetting(bool great = false)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == VillageId);
            switch (TrainingBuilding)
            {
                case BuildingEnums.Barracks:
                    if (great)
                    {
                        setting.IsGreatBarrack = false;
                    }
                    else
                    {
                        setting.BarrackTroop = 0;
                    }
                    break;

                case BuildingEnums.Stable:
                    if (great)
                    {
                        setting.IsGreatStable = false;
                    }
                    else
                    {
                        setting.StableTroop = 0;
                    }
                    break;

                case BuildingEnums.Workshop:
                    setting.WorkshopTroop = 0;
                    break;

                default:
                    break;
            }
            context.Update(setting);
            context.SaveChanges();
        }

        private Result EnterBuilding(bool great)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId);

            var buildingType = great ? TrainingBuilding.GetGreatVersion() : TrainingBuilding;
            var building = buildings.FirstOrDefault(x => x.Type == buildingType);
            return _navigateHelper.GoToBuilding(AccountId, building.Id);
        }
    }

    public class BarrackTrainTroopsTask : TrainTroopsTask
    {
        public BarrackTrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, BuildingEnums.Barracks, cancellationToken)
        {
        }
    }

    public class StableTrainTroopsTask : TrainTroopsTask
    {
        public StableTrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, BuildingEnums.Stable, cancellationToken)
        {
        }
    }

    public class WorkshopTrainTroopsTask : TrainTroopsTask
    {
        public WorkshopTrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, BuildingEnums.Workshop, cancellationToken)
        {
        }
    }
}