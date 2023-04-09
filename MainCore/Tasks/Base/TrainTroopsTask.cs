using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class TrainTroopsTask : VillageBotTask
    {
        protected TroopEnums _troop;
        public TroopEnums Troop => _troop;

        protected bool _greatTrain;
        public bool GreatTrain => _greatTrain;

        public TrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            throw new System.NotImplementedException();
        }

        private Result Update()
        {
            var taskUpdate = new UpdateVillage(VillageId, AccountId, CancellationToken);
            var result = taskUpdate.Execute();
            return result;
        }

        private Result CheckTrainBuilding()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);

            var buildingType = Troop.GetTrainBuilding();
            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var building = villageBuilding.FirstOrDefault(x => x.Type == buildingType);
            if (building is null || building.Level < 1) return Result.Fail(new Skip($"Missing {buildingType} to train {Troop}"));

            if (setting.IsGreatBuilding)
            {
                building = villageBuilding.FirstOrDefault(x => x.Type == buildingType.GetGreatVersion());

                _greatTrain = building is not null && building.Level >= 1;
            }
            return Result.Ok();
        }

        private Result Enter()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var smithy = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
            var result = _navigateHelper.GoToBuilding(AccountId, smithy.Id);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}