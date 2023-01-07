using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Update;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Sim
{
    public class UpgradeBuilding : VillageBotTask
    {
        private readonly IUpgradeBuildingHelper _upgradeBuildingHelper;
        private readonly IUpdateHelper _updateHelper;
        private readonly IPlanManager _planManager;

        public UpgradeBuilding(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();

            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _planManager = Locator.Current.GetService<IPlanManager>();
        }

        public override Result Execute()
        {
            do
            {
                PlanTask buildingTask = null;
                {
                    var result = SelectBuilding();
                    if (result.IsFailed) return Result.Ok();
                    buildingTask = result.Value;
                };
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

                if (IsAutoBuilding(buildingTask)) continue;
                {
                    {
                        var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var result = _updateHelper.UpdateResource(AccountId, VillageId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

                if (!_upgradeBuildingHelper.IsEnoughFreeCrop(VillageId, buildingTask.Building))
                {
                    using var context = _contextFactory.CreateDbContext();
                    var cropland = context.VillagesBuildings.Where(x => x.VillageId == VillageId).Where(x => x.Type == BuildingEnums.Cropland).OrderBy(x => x.Level).FirstOrDefault();
                    var task = new PlanTask()
                    {
                        Type = PlanTypeEnums.General,
                        Level = cropland.Level + 1,
                        Building = BuildingEnums.Cropland,
                        Location = cropland.Id,
                    };
                    _planManager.Insert(VillageId, 0, task);
                    _eventManager.OnVillageBuildQueueUpdate(VillageId);
                }
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                {
                    var result = _upgradeBuildingHelper.IsBuildingCompleted(AccountId, VillageId, buildingTask.Location, buildingTask.Level);
                    if (result.IsFailed)
                    {
                        var newResult = new Result();
                        return newResult.WithErrors(result.Errors).WithError(new Trace(Trace.TraceMessage()));
                    }
                    if (result.Value)
                    {
                        _planManager.Remove(VillageId, buildingTask);
                        _eventManager.OnVillageBuildQueueUpdate(VillageId);
                        continue;
                    }
                }
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                bool isNewBuilding = false;

                {
                    var result = _upgradeBuildingHelper.GotoBuilding(AccountId, VillageId, buildingTask);
                    if (result.IsFailed)
                    {
                        var newResult = new Result();
                        return newResult.WithErrors(result.Errors).WithError(new Trace(Trace.TraceMessage()));
                    }
                    isNewBuilding = result.Value;
                }

                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                if (!_upgradeBuildingHelper.IsEnoughResource(AccountId, VillageId, buildingTask.Building, isNewBuilding))
                {
                    var resMissing = _upgradeBuildingHelper.GetResourceMissing(AccountId, VillageId, buildingTask.Building, isNewBuilding);
                    if (VersionDetector.IsTravianOfficial())
                    {
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
                            var taskUpdate = new UpdateHeroItems(AccountId, CancellationToken);
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

                            var taskEquip = new UseHeroResources(VillageId, AccountId, items, CancellationToken);
                            var result = taskEquip.Execute();
                            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                        }
                        if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                        {
                            var result = _upgradeBuildingHelper.GotoBuilding(AccountId, VillageId, buildingTask);
                            if (result.IsFailed)
                            {
                                var newResult = new Result();
                                return newResult.WithErrors(result.Errors).WithError(new Trace(Trace.TraceMessage()));
                            }
                        }
                    }
                    else if (VersionDetector.IsTTWars())
                    {
                        _logManager.Information(AccountId, "Don't have enough resources.");
                        using var context = _contextFactory.CreateDbContext();
                        var production = context.VillagesProduction.Find(VillageId);
                        var timeEnough = production.GetTimeWhenEnough(resMissing);
                        ExecuteAt = timeEnough;
                        return Result.Ok();
                    }
                }

                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                if (isNewBuilding)
                {
                    var result = Construct(buildingTask);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                else
                {
                    if (VersionDetector.IsTravianOfficial())
                    {
                        var result = _upgradeBuildingHelper.IsNeedAdsUpgrade(AccountId, VillageId, buildingTask);
                        if (result.IsFailed)
                        {
                            var newResult = new Result();
                            return newResult.WithErrors(result.Errors).WithError(new Trace(Trace.TraceMessage()));
                        }

                        if (result.Value)
                        {
                            var upgrade = UpgradeAds(buildingTask);
                            if (upgrade.IsFailed) return upgrade.WithError(new Trace(Trace.TraceMessage()));
                        }
                        else
                        {
                            var upgrade = _upgradeBuildingHelper.Upgrade(AccountId, buildingTask);
                            if (upgrade.IsFailed) return upgrade.WithError(new Trace(Trace.TraceMessage()));
                        }
                    }
                    else if (VersionDetector.IsTTWars())
                    {
                        var upgrade = _upgradeBuildingHelper.Upgrade(AccountId, buildingTask);
                        if (upgrade.IsFailed) return upgrade.WithError(new Trace(Trace.TraceMessage()));
                    }
                }

                Update();
            }
            while (true);
        }

        private Result Construct(PlanTask buildingTask)
        {
            var result = _upgradeBuildingHelper.Construct(AccountId, buildingTask);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (buildingTask.Level == 1)
            {
                _planManager.Remove(VillageId, buildingTask);
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
            }
            return Result.Ok();
        }

        private Result UpgradeAds(PlanTask buildingTask)
        {
            var result = _upgradeBuildingHelper.UpgradeAds(AccountId, buildingTask);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result Update()
        {
            {
                var result = _navigateHelper.WaitPageChanged(AccountId, "dorf");
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var updateTask = new RefreshVillage(VillageId, AccountId);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result<PlanTask> SelectBuilding()
        {
            _upgradeBuildingHelper.RemoveFinishedCB(VillageId);
            var buildingTask = _upgradeBuildingHelper.NextBuildingTask(AccountId, VillageId);
            if (buildingTask.IsFailed)
            {
                _logManager.Information(AccountId, buildingTask.Errors[0].Message);
                var tasks = _planManager.GetList(VillageId);
                if (tasks.Count == 0)
                {
                    return Result.Fail(new Skip());
                }
                if (!_chromeBrowser.GetCurrentUrl().Contains("dorf"))
                {
                    var result = _navigateHelper.GoRandomDorf(AccountId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }

                {
                    var updateTask = new UpdateVillage(VillageId, AccountId);
                    var result = updateTask.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                {
                    using var context = _contextFactory.CreateDbContext();
                    var firstComplete = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Id == 0);
                    if (firstComplete.Level != -1)
                    {
                        var delay = 0;
                        if (VersionDetector.IsTravianOfficial()) delay = 10;
                        else if (VersionDetector.IsTTWars()) delay = 1;
                        ExecuteAt = firstComplete.CompleteTime.AddSeconds(delay);
                        _logManager.Information(AccountId, $"Next building will be contructed after {firstComplete.Type} - level {firstComplete.Level} complete. ({ExecuteAt})");
                    }
                }
                return Result.Fail(new Skip());
            }

            if (buildingTask.Value.Type == PlanTypeEnums.General)
            {
                _logManager.Information(AccountId, $"Next building will be contructed: {buildingTask.Value.Building} - level {buildingTask.Value.Level}.");
            }
            else if (buildingTask.Value.Type == PlanTypeEnums.ResFields)
            {
                _logManager.Information(AccountId, $"Next building will be contructed: {buildingTask.Value.ResourceType} - {buildingTask.Value.BuildingStrategy} - level {buildingTask.Value.Level}.");
            }
            return buildingTask;
        }

        private bool IsAutoBuilding(PlanTask buildingTask)
        {
            if (buildingTask.Type == PlanTypeEnums.ResFields)
            {
                var task = _upgradeBuildingHelper.ExtractResField(VillageId, buildingTask);
                if (task is null)
                {
                    _planManager.Remove(VillageId, buildingTask);
                }
                else
                {
                    _planManager.Insert(VillageId, 0, task);
                }
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
                return true;
            }
            return false;
        }
    }
}