using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.TravianOfficial
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

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.VillagesSettings.Find(VillageId);
                if (!setting.IsUseHeroRes)
                {
                    _logManager.Information(AccountId, "Don't have enough resources.");
                    var production = context.VillagesProduction.Find(VillageId);
                    var timeEnough = production.GetTimeWhenEnough(resMissing);
                    ExecuteAt = timeEnough;
                    return Result.Ok();
                }
            }
            {
                var taskUpdate = _taskFactory.GetUpdateHeroItemsTask(AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                using var context = _contextFactory.CreateDbContext();

                var itemsHero = context.HeroesItems.Where(x => x.AccountId == AccountId);
                var woodAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Wood);
                var clayAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Clay);
                var ironAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Iron);
                var cropAvaliable = itemsHero.FirstOrDefault(x => x.Item == HeroItemEnums.Crop);

                var resAvaliable = new long[] { woodAvaliable?.Count ?? 0, clayAvaliable?.Count ?? 0, ironAvaliable?.Count ?? 0, cropAvaliable?.Count ?? 0 };

                var resLeft = new long[] { resAvaliable[0] - resMissing[0], resAvaliable[1] - resMissing[1], resAvaliable[2] - resMissing[2], resAvaliable[3] - resMissing[3] };
                if (resLeft.Any(x => x <= 0))
                {
                    _logManager.Information(AccountId, "Don't have enough resources.");
                    var production = context.VillagesProduction.Find(VillageId);
                    var timeEnough = production.GetTimeWhenEnough(resMissing);
                    ExecuteAt = timeEnough;
                    return Result.Ok();
                }

                var items = new List<(HeroItemEnums, int)>()
                            {
                                (HeroItemEnums.Wood, (int)resMissing[0]),
                                (HeroItemEnums.Clay, (int)resMissing[1]),
                                (HeroItemEnums.Iron, (int)resMissing[2]),
                                (HeroItemEnums.Crop, (int)resMissing[3]),
                            };

                var taskEquip = _taskFactory.GetUseHeroResourcesTask(VillageId, AccountId, items, CancellationToken);
                var result = taskEquip.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            {
                var result = _upgradeBuildingHelper.GotoBuilding(AccountId, VillageId, _chosenTask);
                if (result.IsFailed)
                {
                    var newResult = new Result();
                    return newResult.WithErrors(result.Errors).WithError(new Trace(Trace.TraceMessage()));
                }
            }

            return Result.Ok();
        }

        protected override void ChangeNextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var firstComplete = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Id == 0);
            if (firstComplete.Level != -1)
            {
                var delay = 10;

                ExecuteAt = firstComplete.CompleteTime.AddSeconds(delay);
                _logManager.Information(AccountId, $"Next building will be contructed after {firstComplete.Type} - level {firstComplete.Level} complete. ({ExecuteAt})");
            }
        }
    }
}