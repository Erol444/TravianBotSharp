using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class UpgradeBuilding : VillageBotTask
    {
        protected readonly IUpgradeBuildingHelper _upgradeBuildingHelper;
        protected readonly IUpdateHelper _updateHelper;
        protected readonly IPlanManager _planManager;
        protected readonly IBuildingsHelper _buildingsHelper;

        protected PlanTask _chosenTask;
        protected bool _isNewBuilding;

        public UpgradeBuilding(int VillageId, int AccountId, CancellationToken cancellationToken = default) : base(VillageId, AccountId, cancellationToken)
        {
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();

            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _planManager = Locator.Current.GetService<IPlanManager>();
            _buildingsHelper = Locator.Current.GetService<IBuildingsHelper>();
        }

        public override Result Execute()
        {
            do
            {
                var commands = new List<Func<Result>>()
                {
                    PreUpdate,
                    ChooseBuilding,
                    CheckBuilding,
                };

                foreach (var command in commands)
                {
                    _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                    var result = command.Invoke();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }

                if (IsAutoResField()) continue;

                commands = new List<Func<Result>>()
                {
                    Update,
                };

                foreach (var command in commands)
                {
                    _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                    var result = command.Invoke();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }

                if (IsCompleted()) continue;

                if (!IsEnoughFreeCrop()) continue;

                commands = new List<Func<Result>>()
                {
                    GotoBuilding,
                    GotoCorrectTab,
                    CheckResource
                };

                foreach (var command in commands)
                {
                    _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                    var result = command.Invoke();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }

                if (_isNewBuilding)
                {
                    var result = Construct(_chosenTask);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                else
                {
                    var result = _upgradeBuildingHelper.IsNeedAdsUpgrade(AccountId, VillageId, _chosenTask);
                    if (result.IsFailed)
                    {
                        var newResult = new Result();
                        return newResult.WithErrors(result.Errors).WithError(new Trace(Trace.TraceMessage()));
                    }

                    if (result.Value)
                    {
                        var upgrade = UpgradeAds(_chosenTask);
                        if (upgrade.IsFailed) return upgrade.WithError(new Trace(Trace.TraceMessage()));
                    }
                    else
                    {
                        var upgrade = _upgradeBuildingHelper.Upgrade(AccountId, _chosenTask);
                        if (upgrade.IsFailed) return upgrade.WithError(new Trace(Trace.TraceMessage()));
                    }
                }

                commands = new List<Func<Result>>()
                {
                    PostUpdate,
                };

                foreach (var command in commands)
                {
                    _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                    var result = command.Invoke();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }
            } while (true);
        }

        protected Result Construct(PlanTask buildingTask)
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

        protected Result UpgradeAds(PlanTask buildingTask)
        {
            var result = _upgradeBuildingHelper.UpgradeAds(AccountId, buildingTask);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected Result PreUpdate()
        {
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
            return Result.Ok();
        }

        protected Result ChooseBuilding()
        {
            _chosenTask = null;
            _upgradeBuildingHelper.RemoveFinishedCB(VillageId);

            var tasks = _planManager.GetList(VillageId);
            if (tasks.Count == 0)
            {
                return Result.Fail(new Skip("Queue is empty."));
            }

            using var context = _contextFactory.CreateDbContext();
            var currentList = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId && x.Level != -1).ToList();
            var totalBuild = currentList.Count;

            if (totalBuild == 0)
            {
                _chosenTask = _upgradeBuildingHelper.GetFirstTask(VillageId);
                return Result.Ok();
            }

            var accountInfo = context.AccountsInfo.Find(AccountId);
            var tribe = accountInfo.Tribe;
            var hasPlusAccount = accountInfo.HasPlusAccount;
            var setting = context.VillagesSettings.Find(VillageId);

            var romanAdvantage = tribe == TribeEnums.Romans && !setting.IsIgnoreRomanAdvantage;

            var maxBuild = 1;
            if (hasPlusAccount) maxBuild++;
            if (romanAdvantage) maxBuild++;
            if (totalBuild == maxBuild)
            {
                ChangeNextExecute();
                return Result.Fail(new Skip("Amount of currently building is equal with maximum building can build in same time"));
            }

            // roman has special queue
            if (!romanAdvantage)
            {
                _chosenTask = _upgradeBuildingHelper.GetFirstTask(VillageId);
                return Result.Ok();
            }

            // there is atleast 2 slot free
            // roman can build both building or resource field
            if (maxBuild - totalBuild >= 2)
            {
                _chosenTask = _upgradeBuildingHelper.GetFirstTask(VillageId);
                return Result.Ok();
            }

            var numQueueRes = tasks.Count(x => x.Building.IsResourceField() || x.Type == PlanTypeEnums.ResFields);
            var numQueueBuilding = tasks.Count - numQueueRes;

            var numCurrentRes = currentList.Count(x => x.Type.IsResourceField());
            var numCurrentBuilding = totalBuild - numCurrentRes;

            if (numCurrentRes > numCurrentBuilding)
            {
                var freeCrop = context.VillagesResources.Find(VillageId).FreeCrop;
                if (freeCrop < 6)
                {
                    ChangeNextExecute();
                    return Result.Fail(new Skip("Cannot build because of lack of freecrop ( < 6 )"));
                }

                if (numQueueBuilding == 0)
                {
                    ChangeNextExecute();
                    return Result.Fail(new Skip("There is no building task in queue"));
                }

                _chosenTask = _upgradeBuildingHelper.GetFirstBuildingTask(VillageId);
                return Result.Ok();
            }

            if (numCurrentBuilding > numCurrentRes)
            {
                // no need check free crop, there is magic make sure this always choose crop
                // jk, because of how we check free crop later, first res task is always crop
                if (numQueueRes == 0)
                {
                    ChangeNextExecute();
                    return Result.Fail(new Skip("There is no res field task in queue"));
                }
                _chosenTask = _upgradeBuildingHelper.GetFirstResTask(VillageId);
                return Result.Ok();
            }
            // if same means 1 R and 1 I already, 1 ANY will be choose below
            else
            {
                _chosenTask = _upgradeBuildingHelper.GetFirstTask(VillageId);
                return Result.Ok();
            }
        }

        protected Result CheckBuilding()
        {
            if (_chosenTask is null)
            {
                ExecuteAt = DateTime.Now.AddSeconds(15);
                _logManager.Information(AccountId, "There is somthing wrong, bot cannot decide what to build. Let wait bot in 15 secs");
                return Result.Fail(new Retry("Cannot chose building to build"));
            }

            if (_chosenTask.Type == PlanTypeEnums.General)
            {
                _logManager.Information(AccountId, $"Next building will be contructed: {_chosenTask.Building} - level {_chosenTask.Level}.");
            }
            else if (_chosenTask.Type == PlanTypeEnums.ResFields)
            {
                _logManager.Information(AccountId, $"Next building will be contructed: {_chosenTask.ResourceType} - {_chosenTask.BuildingStrategy} - level {_chosenTask.Level}.");
            }
            return Result.Ok();
        }

        protected bool IsAutoResField()
        {
            if (_chosenTask.Type != PlanTypeEnums.ResFields) return false;

            var task = _upgradeBuildingHelper.ExtractResField(VillageId, _chosenTask);
            if (task is null)
            {
                _planManager.Remove(VillageId, _chosenTask);
            }
            else
            {
                _planManager.Insert(VillageId, 0, task);
            }
            _eventManager.OnVillageBuildQueueUpdate(VillageId);
            return true;
        }

        protected Result Update()
        {
            using var context = _contextFactory.CreateDbContext();
            // move to correct page
            var dorf = _buildingsHelper.GetDorf(_chosenTask.Building);
            var chrromeBroser = _chromeManager.Get(AccountId);
            var url = chrromeBroser.GetCurrentUrl();
            switch (dorf)
            {
                case 1:
                    {
                        var taskUpdate = new UpdateDorf1(VillageId, AccountId);
                        var result = taskUpdate.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    break;

                case 2:
                    {
                        var taskUpdate = new UpdateDorf2(VillageId, AccountId);
                        var result = taskUpdate.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    break;
            }

            return Result.Ok();
        }

        protected bool IsEnoughFreeCrop()
        {
            if (_upgradeBuildingHelper.IsEnoughFreeCrop(VillageId, _chosenTask.Building)) return true;

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
            return false;
        }

        protected bool IsCompleted()
        {
            var buildingId = _chosenTask.Location;
            var buildingLevel = _chosenTask.Level;
            using var context = _contextFactory.CreateDbContext();
            var building = context.VillagesBuildings.FirstOrDefault(x => x.VillageId == VillageId && x.Id == buildingId);
            if (building.Level >= buildingLevel)
            {
                _planManager.Remove(VillageId, _chosenTask);
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
                return true;
            }

            var currently = context.VillagesCurrentlyBuildings.FirstOrDefault(x => x.VillageId == VillageId && x.Location == buildingId);
            if (currently is not null && currently.Level >= buildingLevel)
            {
                _planManager.Remove(VillageId, _chosenTask);
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
                return true;
            }

            return false;
        }

        protected Result GotoBuilding()
        {
            var result = _navigateHelper.GoToBuilding(AccountId, _chosenTask.Location);
            return result;
        }

        protected Result GotoCorrectTab()
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.VillagesBuildings.Find(VillageId, _chosenTask.Location);

            bool isNewBuilding = false;
            if (building.Type == BuildingEnums.Site)
            {
                isNewBuilding = true;
                var tab = _chosenTask.Building.GetBuildingsCategory();
                {
                    var result = _navigateHelper.SwitchTab(AccountId, tab);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else
            {
                if (building.Level == -1)
                {
                    isNewBuilding = true;
                }
                else
                {
                    if (_chosenTask.Building.HasMultipleTabs() && building.Level != 0)
                    {
                        var result = _navigateHelper.SwitchTab(AccountId, 0);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
            }

            _isNewBuilding = isNewBuilding;
            return Result.Ok();
        }

        protected abstract Result CheckResource();

        protected Result PostUpdate()
        {
            {
                var result = _navigateHelper.WaitPageChanged(AccountId, "dorf");
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var updateTask = _taskFactory.GetRefreshVillageTask(VillageId, AccountId);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        protected abstract void ChangeNextExecute();
    }
}