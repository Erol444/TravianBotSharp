using FluentResults;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.TTWars
{
    public class UpgradeBuilding : Base.UpgradeBuilding
    {
        public UpgradeBuilding(int VillageId, int AccountId, CancellationToken cancellationToken = default) : base(VillageId, AccountId, cancellationToken)
        {
        }

        protected override Result CheckResource()
        {
            if (_upgradeBuildingHelper.IsEnoughResource(AccountId, VillageId, _chosenTask.Building, _isNewBuilding)) return Result.Ok();

            var resMissing = _upgradeBuildingHelper.GetResourceMissing(AccountId, VillageId, _chosenTask.Building, _isNewBuilding);

            _logManager.Information(AccountId, "Don't have enough resources.");
            using var context = _contextFactory.CreateDbContext();
            var production = context.VillagesProduction.Find(VillageId);
            var timeEnough = production.GetTimeWhenEnough(resMissing);
            ExecuteAt = timeEnough;
            return Result.Ok();
        }

        protected override void ChangeNextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var firstComplete = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Id == 0);
            if (firstComplete.Level != -1)
            {
                var delay = 1;
                ExecuteAt = firstComplete.CompleteTime.AddSeconds(delay);
                _logManager.Information(AccountId, $"Next building will be contructed after {firstComplete.Type} - level {firstComplete.Level} complete. ({ExecuteAt})");
            }
        }
    }
}