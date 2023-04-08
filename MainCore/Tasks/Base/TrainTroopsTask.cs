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

        public TrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public TroopEnums Troop => _troop;

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

        private Result CheckSmithy()
        {
            if (!IsVaild())
            {
                return Result.Fail(new Skip("Missing smithy"));
            }
            return Result.Ok();
        }

        private bool IsVaild()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var troop = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
            if (troop is null) return false;
            if (troop.Level <= 0) return false;
            return true;
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