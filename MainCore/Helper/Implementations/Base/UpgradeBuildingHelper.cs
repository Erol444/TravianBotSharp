using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class UpgradeBuildingHelper : IUpgradeBuildingHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IPlanManager _planManager;
        protected readonly IChromeManager _chromeManager;
        protected readonly ISystemPageParser _systemPageParser;
        protected readonly IBuildingsHelper _buildingsHelper;
        protected readonly IGeneralHelper _generalHelper;
        protected readonly IHeroResourcesHelper _heroResourcesHelper;
        protected readonly ILogManager _logManager;
        protected readonly IEventManager _eventManager;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        protected PlanTask _chosenTask;
        protected bool _isNewBuilding;

        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IBuildingsHelper buildingsHelper, IGeneralHelper generalHelper, ILogManager logManager, IEventManager eventManager, IHeroResourcesHelper heroResourcesHelper)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
            _chromeManager = chromeManager;
            _systemPageParser = systemPageParser;
            _buildingsHelper = buildingsHelper;
            _generalHelper = generalHelper;
            _logManager = logManager;
            _eventManager = eventManager;
            _heroResourcesHelper = heroResourcesHelper;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);

            _heroResourcesHelper.Load(villageId, accountId, cancellationToken);
        }

        public Result Execute()
        {
            _result = _generalHelper.SwitchVillage(_accountId, _villageId);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.ToDorf1(_accountId, forceReload: true);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            while (true)
            {
                #region choose building

                _result = ChooseBuilding();
                if (_result.IsFailed)
                {
                    //if (_result.HasError<NoTaskInQueue>() || _result.HasError<QueueFull>())
                    if (_result.HasError<LackFreeCrop>())
                    {
                        using var context = _contextFactory.CreateDbContext();
                        var cropland = context.VillagesBuildings.Where(x => x.VillageId == _villageId).Where(x => x.Type == BuildingEnums.Cropland).OrderBy(x => x.Level).FirstOrDefault();
                        var task = new PlanTask()
                        {
                            Type = PlanTypeEnums.General,
                            Level = cropland.Level + 1,
                            Building = BuildingEnums.Cropland,
                            Location = cropland.Id,
                        };
                        _planManager.Insert(_villageId, 0, task);
                        _eventManager.OnVillageBuildQueueUpdate(_villageId);
                        continue;
                    }

                    return _result.WithError(new Trace(Trace.TraceMessage()));
                }

                #endregion choose building

                #region extract resource fields

                if (_chosenTask.Type == PlanTypeEnums.ResFields)
                {
                    var task = ExtractResField();
                    if (task is null)
                    {
                        _planManager.Remove(_villageId, _chosenTask);
                    }
                    else
                    {
                        _planManager.Insert(_villageId, 0, task);
                    }
                    _eventManager.OnVillageBuildQueueUpdate(_villageId);
                    continue;
                }

                #endregion extract resource fields

                #region enter building

                _result = _generalHelper.ToBuilding(_accountId, _chosenTask.Location);
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
                _result = GotoCorrectTab();
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

                #endregion enter building

                _result = CheckResource();
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

                #region click

                if (_isNewBuilding)
                {
                    Construct();
                }
                else
                {
                    if (IsNeedAdsUpgrade())
                    {
                        _result = UpgradeAds();
                        if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    else
                    {
                        _result = Upgrade();
                        if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }

                #endregion click
            }
        }

        protected PlanTask ExtractResField()
        {
            List<VillageBuilding> buildings = null; // Potential buildings to be upgraded next
            using var context = _contextFactory.CreateDbContext();
            switch (_chosenTask.ResourceType)
            {
                case ResTypeEnums.AllResources:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == _villageId && (x.Type == BuildingEnums.Woodcutter || x.Type == BuildingEnums.ClayPit || x.Type == BuildingEnums.IronMine || x.Type == BuildingEnums.Cropland)).ToList();
                    break;

                case ResTypeEnums.ExcludeCrop:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == _villageId && (x.Type == BuildingEnums.Woodcutter || x.Type == BuildingEnums.ClayPit || x.Type == BuildingEnums.IronMine)).ToList();
                    break;

                case ResTypeEnums.OnlyCrop:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == _villageId && x.Type == BuildingEnums.Cropland).ToList();
                    break;
            }

            foreach (var b in buildings)
            {
                if (b.IsUnderConstruction)
                {
                    var levelUpgrading = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId).Count(x => x.Location == b.Id);
                    b.Level += (byte)levelUpgrading;
                }
            }
            buildings = buildings.Where(b => b.Level < _chosenTask.Level).ToList();

            if (buildings.Count == 0) return null;

            var building = buildings.OrderBy(x => x.Level).FirstOrDefault();

            return new()
            {
                Type = PlanTypeEnums.General,
                Level = building.Level + 1,
                Building = building.Type,
                Location = building.Id,
            };
        }

        public void RemoveFinishedCB()
        {
            using var context = _contextFactory.CreateDbContext();
            var tasksDone = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId && x.CompleteTime <= DateTime.Now);

            if (!tasksDone.Any()) return;

            foreach (var taskDone in tasksDone)
            {
                var building = context.VillagesBuildings.Find(_villageId, taskDone.Location);
                if (building is null)
                {
                    building = context.VillagesBuildings.FirstOrDefault(x => x.VillageId == _villageId && x.Type == taskDone.Type);
                    if (building is null) continue;
                }

                if (building.Level < taskDone.Level) building.Level = taskDone.Level;

                taskDone.Type = 0;
                taskDone.Level = -1;
                taskDone.CompleteTime = DateTime.MaxValue;

                context.Update(taskDone);
                context.Update(building);
            }
            context.SaveChanges();
        }

        protected PlanTask GetFirstResTask()
        {
            var tasks = _planManager.GetList(_villageId);
            var task = tasks.FirstOrDefault(x => x.Type == PlanTypeEnums.ResFields || x.Building.IsResourceField());
            return task;
        }

        protected PlanTask GetFirstBuildingTask()
        {
            var tasks = _planManager.GetList(_villageId);
            var infrastructureTasks = tasks.Where(x => x.Type == PlanTypeEnums.General && !x.Building.IsResourceField());
            var task = infrastructureTasks.FirstOrDefault(x => IsInfrastructureTaskVaild(x));
            return task;
        }

        protected PlanTask GetFirstTask()
        {
            var tasks = _planManager.GetList(_villageId);
            foreach (var task in tasks)
            {
                if (task.Type != PlanTypeEnums.General) return task;
                if (task.Building.IsResourceField()) return task;
                if (IsInfrastructureTaskVaild(task)) return task;
            }
            return null;
        }

        private bool IsInfrastructureTaskVaild(PlanTask planTask)
        {
            (_, var prerequisiteBuildings) = planTask.Building.GetPrerequisiteBuildings();
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == _villageId).ToList();
            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var building = buildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Type == prerequisiteBuilding.Building);
                if (building is null) return false;
                if (building.Level < prerequisiteBuilding.Level) return false;
            }
            return true;
        }

        protected bool IsNeedAdsUpgrade()
        {
            if (VersionDetector.GetVersion() != VersionEnums.TravianOfficial) return false;
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(_villageId);
            if (!setting.IsAdsUpgrade) return false;

            var building = context.VillagesBuildings.Find(_villageId, _chosenTask.Location);

            if (_chosenTask.Building.IsResourceField() && building.Level == 0) return false;
            if (_chosenTask.Building.IsNotAdsUpgrade()) return false;

            var html = _chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));

            var durationNode = container.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            if (durationNode is null)
            {
                return false;
            }
            var dur = durationNode.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
            if (dur is null)
            {
                return false;
            }
            var duration = dur.InnerText.ToDuration();
            if (setting.AdsUpgradeTime > duration.TotalMinutes) return false;
            return true;
        }

        protected Result CheckResource()
        {
            using var context = _contextFactory.CreateDbContext();

            var resNeed = GetResourceNeed(_chosenTask.Building, _isNewBuilding);
            var resCurrent = context.VillagesResources.Find(_villageId);
            if (resNeed < resCurrent) return Result.Ok();

            var setting = context.VillagesSettings.Find(_villageId);
            if (!setting.IsUseHeroRes) return Result.Fail(NoResource.Build(_chosenTask.Building, _chosenTask.Level));

            var buildingUrl = _chromeBrowser.GetCurrentUrl();

            _result = _generalHelper.ToHeroInventory(_accountId);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _heroResourcesHelper.FillResource(resCurrent - resNeed);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Navigate(_accountId, buildingUrl);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        protected Resources GetResourceNeed(BuildingEnums building, bool multiple = false)
        {
            var html = _chromeBrowser.GetHtml();

            HtmlNode contractNode;
            if (multiple && !building.IsResourceField())
            {
                contractNode = html.GetElementbyId($"contract_building{(int)building}");
            }
            else
            {
                contractNode = _systemPageParser.GetContractNode(html);
            }
            var resWrapper = contractNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var resNodes = resWrapper.ChildNodes.Where(x => x.HasClass("resource") || x.HasClass("resources")).ToList();
            var resNeed = new long[4];
            for (var i = 0; i < 4; i++)
            {
                var node = resNodes[i];
                var strResult = new string(node.InnerText.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(strResult)) resNeed[i] = 0;
                else resNeed[i] = long.Parse(strResult);
            }
            return new Resources(resNeed);
        }

        protected bool IsEnoughFreeCrop(BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var freecrop = context.VillagesResources.Find(_villageId).FreeCrop;
            return freecrop > 5 || building == BuildingEnums.Cropland;
        }

        protected Result GotoCorrectTab()
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.VillagesBuildings.Find(_villageId, _chosenTask.Location);

            bool isNewBuilding = false;
            if (building.Type == BuildingEnums.Site)
            {
                isNewBuilding = true;
                var tab = _chosenTask.Building.GetBuildingsCategory();
                {
                    var result = _generalHelper.SwitchTab(_accountId, tab);
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
                        var result = _generalHelper.SwitchTab(_accountId, 0);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
            }

            _isNewBuilding = isNewBuilding;
            return Result.Ok();
        }

        protected Result Construct()
        {
            var html = _chromeBrowser.GetHtml();
            var node = html.GetElementbyId($"contract_building{(int)_chosenTask.Building}");
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find building box"));
            }
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button is null)
            {
                return Result.Fail(new Retry($"Cannot find Build button for {_chosenTask.Building}"));
            }
            _result = _generalHelper.Click(_accountId, By.XPath(button.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected Result Upgrade()
        {
            var html = _chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            if (container is null)
            {
                return Result.Fail(new Retry("Cannot find upgrading box"));
            }
            var upgradeButton = container.Descendants("button").FirstOrDefault(x => x.HasClass("build"));

            if (upgradeButton == null)
            {
                return Result.Fail(new Retry($"Cannot find upgrade button for {_chosenTask.Building}"));
            }

            _result = _generalHelper.Click(_accountId, By.XPath(upgradeButton.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private Result UpgradeAds_UpgradeClicking()
        {
            var html = _chromeBrowser.GetHtml();

            var nodeFastUpgrade = html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("videoFeatureButton") && x.HasClass("green"));
            if (nodeFastUpgrade is null)
            {
                return Result.Fail(new Retry($"Cannot find fast upgrade button for {_chosenTask.Building}"));
            }

            _result = _generalHelper.Click(_accountId, By.XPath(nodeFastUpgrade.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private Result UpgradeAds_AccpetAds()
        {
            var html = _chromeBrowser.GetHtml();
            var nodeNotShowAgainConfirm = html.DocumentNode.SelectSingleNode("//input[@name='adSalesVideoInfoScreen']");
            if (nodeNotShowAgainConfirm is not null)
            {
                _result = _generalHelper.Click(_accountId, By.XPath(nodeNotShowAgainConfirm.ParentNode.XPath));
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
                _chromeBrowser.GetChrome().ExecuteScript("jQuery(window).trigger('showVideoWindowAfterInfoScreen')");
            }
            return Result.Ok();
        }

        private void UpgradeAds_CloseOtherTab()
        {
            var chrome = _chromeBrowser.GetChrome();
            var current = chrome.CurrentWindowHandle;
            while (chrome.WindowHandles.Count > 1)
            {
                var others = chrome.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                chrome.SwitchTo().Window(others);
                chrome.Close();
                chrome.SwitchTo().Window(current);
            }
        }

        private Result UpgradeAds_ClickPlayAds()
        {
            var html = _chromeBrowser.GetHtml();
            var nodeIframe = html.GetElementbyId("videoFeature");
            if (nodeIframe is null)
            {
                return Result.Fail(new Retry($"Cannot find iframe for {_chosenTask.Building}"));
            }

            _result = _generalHelper.Click(_accountId, By.XPath(nodeIframe.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            var chrome = _chromeBrowser.GetChrome();
            chrome.SwitchTo().DefaultContent();

            Thread.Sleep(Random.Shared.Next(1300, 2000));
            // close if bot click on playing ads
            // chrome will open new tab & pause ads
            do
            {
                var handles = chrome.WindowHandles;
                if (handles.Count == 1) break;

                var current = chrome.CurrentWindowHandle;
                var other = chrome.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                chrome.SwitchTo().Window(other);
                chrome.Close();
                chrome.SwitchTo().Window(current);

                _result = _generalHelper.Click(_accountId, By.XPath(nodeIframe.XPath), waitPageLoaded: false);
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

                chrome.SwitchTo().DefaultContent();
            }
            while (true);
            return Result.Ok();
        }

        private Result UpgradeAds_DontShowThis()
        {
            var html = _chromeBrowser.GetHtml();
            if (html.GetElementbyId("dontShowThisAgain") is not null)
            {
                _result = _generalHelper.Click(_accountId, By.Id("dontShowThisAgain"));
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

                _result = _generalHelper.Click(_accountId, By.ClassName("dialogButtonOk"));
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result UpgradeAds()
        {
            _result = UpgradeAds_UpgradeClicking();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            Thread.Sleep(Random.Shared.Next(2400, 5300));

            _result = UpgradeAds_AccpetAds();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            Thread.Sleep(Random.Shared.Next(20000, 25000));

            UpgradeAds_CloseOtherTab();

            _result = UpgradeAds_ClickPlayAds();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.WaitPageChanged(_accountId, "dorf");
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = UpgradeAds_DontShowThis();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected Result ChooseBuilding()
        {
            _chosenTask = null;
            var tasks = _planManager.GetList(_villageId);
            if (tasks.Count == 0)
            {
                return Result.Fail(new Skip("Queue is empty."));
            }

            using var context = _contextFactory.CreateDbContext();
            var currentList = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId && x.Level != -1).ToList();
            var totalBuild = currentList.Count;

            if (totalBuild == 0)
            {
                _chosenTask = GetFirstTask();
                return Result.Ok();
            }

            var accountInfo = context.AccountsInfo.Find(_accountId);
            var tribe = accountInfo.Tribe;
            var hasPlusAccount = accountInfo.HasPlusAccount;
            var setting = context.VillagesSettings.Find(_villageId);

            var romanAdvantage = tribe == TribeEnums.Romans && !setting.IsIgnoreRomanAdvantage;

            var maxBuild = 1;
            if (hasPlusAccount) maxBuild++;
            if (romanAdvantage) maxBuild++;
            if (totalBuild == maxBuild)
            {
                return Result.Fail(new QueueFull());
            }

            // there is atleast 2 slot free
            // roman can build both building or resource field
            if (maxBuild - totalBuild >= 2)
            {
                _chosenTask = GetFirstTask();
                return Result.Ok();
            }

            var numQueueRes = tasks.Count(x => x.Building.IsResourceField() || x.Type == PlanTypeEnums.ResFields);
            var numQueueBuilding = tasks.Count - numQueueRes;

            var numCurrentRes = currentList.Count(x => x.Type.IsResourceField());
            var numCurrentBuilding = totalBuild - numCurrentRes;

            if (numCurrentRes > numCurrentBuilding)
            {
                var freeCrop = context.VillagesResources.Find(_villageId).FreeCrop;
                if (freeCrop < 6)
                {
                    return Result.Fail(new LackFreeCrop());
                }

                if (numQueueBuilding == 0)
                {
                    return Result.Fail(NoTaskInQueue.Building);
                }

                _chosenTask = GetFirstBuildingTask();
                return Result.Ok();
            }

            if (numCurrentBuilding > numCurrentRes)
            {
                // no need check free crop, there is magic make sure this always choose crop
                // jk, because of how we check free crop later, first res task is always crop
                if (numQueueRes == 0)
                {
                    return Result.Fail(NoTaskInQueue.Resource);
                }

                _chosenTask = GetFirstResTask();
                return Result.Ok();
            }

            // if same means 1 R and 1 I already, 1 ANY will be choose below
            _chosenTask = GetFirstTask();
            return Result.Ok();
        }
    }
}