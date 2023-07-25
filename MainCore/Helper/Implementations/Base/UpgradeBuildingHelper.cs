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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;
        private readonly IChromeManager _chromeManager;
        private readonly ISystemPageParser _systemPageParser;
        private readonly IGeneralHelper _generalHelper;
        private readonly IUpdateHelper _updateHelper;
        private readonly IHeroResourcesHelper _heroResourcesHelper;
        private readonly IEventManager _eventManager;
        private readonly IBuildingsHelper _buildingsHelper;
        private readonly IDatabaseHelper _databaseHelper;
        private readonly ILogHelper _logHelper;

        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IGeneralHelper generalHelper, IEventManager eventManager, IHeroResourcesHelper heroResourcesHelper, IUpdateHelper updateHelper, IBuildingsHelper buildingsHelper, IDatabaseHelper databaseHelper, ILogHelper logHelper)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
            _chromeManager = chromeManager;
            _systemPageParser = systemPageParser;
            _generalHelper = generalHelper;
            _eventManager = eventManager;
            _heroResourcesHelper = heroResourcesHelper;
            _updateHelper = updateHelper;
            _buildingsHelper = buildingsHelper;
            _databaseHelper = databaseHelper;
            _logHelper = logHelper;
        }

        public abstract DateTime GetNextExecute(DateTime completeTime);

        public Result Execute(int accountId, int villageId)
        {
            Result result;

            // update currently building
            result = _generalHelper.ToDorf(accountId, villageId, forceReload: true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            while (true)
            {
                #region choose building

                var resultBuilding = ChooseBuilding(accountId, villageId);
                if (resultBuilding.IsFailed)
                {
                    if (resultBuilding.HasError<LackFreeCrop>())
                    {
                        var cropland = _databaseHelper.GetCropLand(villageId);
                        var task = new PlanTask()
                        {
                            Type = PlanTypeEnums.General,
                            Level = cropland.Level + 1,
                            Building = BuildingEnums.Cropland,
                            Location = cropland.Id,
                        };
                        _planManager.Insert(villageId, 0, task);
                        _eventManager.OnVillageBuildQueueUpdate(villageId);
                        continue;
                    }

                    return Result.Fail(resultBuilding.Errors).WithError(new Trace(Trace.TraceMessage()));
                }

                #endregion choose building

                var chosenTask = resultBuilding.Value;

                #region extract resource fields

                if (chosenTask.Type == PlanTypeEnums.ResFields)
                {
                    var task = ExtractResField(villageId, chosenTask);
                    if (task is null)
                    {
                        _planManager.Remove(villageId, chosenTask);
                    }
                    else
                    {
                        _planManager.Insert(villageId, 0, task);
                        _logHelper.Information(accountId, $"Imported {task.Building} - level {task.Level} to build queue from {chosenTask.Content}.");
                    }
                    _eventManager.OnVillageBuildQueueUpdate(villageId);

                    continue;
                }

                #endregion extract resource fields

                #region validate plan task

                result = _generalHelper.ToDorf(accountId, villageId, chosenTask.Location);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                var isTaskComplete = _buildingsHelper.IsTaskComplete(villageId, chosenTask);
                if (isTaskComplete)
                {
                    _planManager.Remove(villageId, chosenTask);
                    _eventManager.OnVillageBuildQueueUpdate(villageId);
                    continue;
                }

                var isPrerequisiteAvailableResult = _buildingsHelper.IsPrerequisiteAvailable(villageId, chosenTask);
                if (isPrerequisiteAvailableResult.IsFailed) return Result.Fail(isPrerequisiteAvailableResult.Errors).WithError(new Trace(Trace.TraceMessage()));
                var isPrerequisiteAvailable = isPrerequisiteAvailableResult.Value;
                if (!isPrerequisiteAvailable)
                {
                    return Result.Fail(new Skip($"{chosenTask.Building} - level {chosenTask.Level} doens't have enough prerequisite buildings. Please check your build queue."));
                }

                var isMultipleReadyResult = _buildingsHelper.IsMultipleReady(villageId, chosenTask);
                if (isMultipleReadyResult.IsFailed) return Result.Fail(isMultipleReadyResult.Errors).WithError(new Trace(Trace.TraceMessage()));
                var isMultipleReady = isMultipleReadyResult.Value;
                if (!isMultipleReady)
                {
                    return Result.Fail(new Skip($"{chosenTask.Building} - level {chosenTask.Level} is going to build but the first {chosenTask.Building} isn't max level. Please check your build queue."));
                }

                #endregion validate plan task

                #region enter building

                result = _generalHelper.ToBuilding(accountId, villageId, chosenTask.Location);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                var resultIsNewBuilding = GotoCorrectTab(accountId, villageId, chosenTask);
                if (resultIsNewBuilding.IsFailed) return Result.Fail(resultIsNewBuilding.Errors).WithError(new Trace(Trace.TraceMessage()));

                #endregion enter building

                var isNewBuilding = resultIsNewBuilding.Value;

                result = CheckResource(accountId, villageId, chosenTask, isNewBuilding);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                #region click

                if (isNewBuilding)
                {
                    _logHelper.Information(accountId, $"Start building {chosenTask.Building} - level {chosenTask.Level}.");
                    Construct(accountId, chosenTask);
                }
                else
                {
                    _logHelper.Information(accountId, $"Start upgrade {chosenTask.Building} - level {chosenTask.Level}.");
                    if (IsNeedAdsUpgrade(accountId, villageId, chosenTask))
                    {
                        result = UpgradeAds(accountId, chosenTask);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    else
                    {
                        result = Upgrade(accountId, chosenTask);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }

                #endregion click

                result = _updateHelper.UpdateCurrentDorf(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
        }

        public PlanTask ExtractResField(int villageId, PlanTask task)
        {
            List<VillageBuilding> buildings = null; // Potential buildings to be upgraded next
            using var context = _contextFactory.CreateDbContext();
            switch (task.ResourceType)
            {
                case ResTypeEnums.AllResources:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId && (x.Type == BuildingEnums.Woodcutter || x.Type == BuildingEnums.ClayPit || x.Type == BuildingEnums.IronMine || x.Type == BuildingEnums.Cropland)).ToList();
                    break;

                case ResTypeEnums.ExcludeCrop:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId && (x.Type == BuildingEnums.Woodcutter || x.Type == BuildingEnums.ClayPit || x.Type == BuildingEnums.IronMine)).ToList();
                    break;

                case ResTypeEnums.OnlyCrop:
                    buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId && x.Type == BuildingEnums.Cropland).ToList();
                    break;
            }

            foreach (var b in buildings)
            {
                if (b.IsUnderConstruction)
                {
                    var levelUpgrading = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Count(x => x.Location == b.Id);
                    b.Level += (byte)levelUpgrading;
                }
            }
            buildings = buildings.Where(b => b.Level < task.Level).ToList();

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

        public void RemoveFinishedCB(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var tasksDone = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.CompleteTime <= DateTime.Now);

            if (!tasksDone.Any()) return;

            foreach (var taskDone in tasksDone)
            {
                var building = context.VillagesBuildings.Find(villageId, taskDone.Location);
                if (building is null)
                {
                    building = context.VillagesBuildings.FirstOrDefault(x => x.VillageId == villageId && x.Type == taskDone.Type);
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

        public PlanTask GetFirstResTask(int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            var task = tasks.FirstOrDefault(x => x.Type == PlanTypeEnums.ResFields || x.Building.IsResourceField());
            return task;
        }

        public PlanTask GetFirstBuildingTask(int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            var infrastructureTasks = tasks.Where(x => x.Type == PlanTypeEnums.General && !x.Building.IsResourceField());
            var task = infrastructureTasks.FirstOrDefault(x => IsInfrastructureTaskVaild(villageId, x));
            return task;
        }

        public PlanTask GetFirstTask(int villageId)
        {
            var tasks = _planManager.GetList(villageId);
            return tasks.FirstOrDefault();
        }

        public bool IsInfrastructureTaskVaild(int villageId, PlanTask planTask)
        {
            (_, var prerequisiteBuildings) = planTask.Building.GetPrerequisiteBuildings();
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).ToList();
            foreach (var prerequisiteBuilding in prerequisiteBuildings)
            {
                var building = buildings.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Type == prerequisiteBuilding.Building);
                if (building is null) return false;
                if (building.Level < prerequisiteBuilding.Level) return false;
            }
            return true;
        }

        public bool IsNeedAdsUpgrade(int accountId, int villageId, PlanTask task)
        {
            if (VersionDetector.GetVersion() != VersionEnums.TravianOfficial) return false;
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(villageId);
            if (!setting.IsAdsUpgrade) return false;

            var building = context.VillagesBuildings.Find(villageId, task.Location);

            if (task.Building.IsResourceField() && building.Level == 0) return false;
            if (task.Building.IsNotAdsUpgrade()) return false;

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
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

        public Result CheckResource(int accountId, int villageId, PlanTask task, bool isNewBuilding)
        {
            using var context = _contextFactory.CreateDbContext();

            var resNeed = GetResourceNeed(accountId, task.Building, isNewBuilding);
            var resCurrent = context.VillagesResources.Find(villageId);
            if (resNeed < resCurrent) return Result.Ok();

            var setting = context.VillagesSettings.Find(villageId);
            if (!setting.IsUseHeroRes) return Result.Fail(NoResource.Build(task.Building, task.Level));

            var chromeBrowser = _chromeManager.Get(accountId);
            var buildingUrl = chromeBrowser.GetCurrentUrl();

            var result = _generalHelper.ToHeroInventory(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _heroResourcesHelper.FillResource(accountId, villageId, resNeed - resCurrent);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Navigate(accountId, buildingUrl);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Resources GetResourceNeed(int accountId, BuildingEnums building, bool multiple = false)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

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

        public bool IsEnoughFreeCrop(int villageId, BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var freecrop = context.VillagesResources.Find(villageId).FreeCrop;
            return freecrop > 5 || building == BuildingEnums.Cropland;
        }

        public Result<bool> GotoCorrectTab(int accountId, int villageId, PlanTask task)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.VillagesBuildings.Find(villageId, task.Location);

            bool isNewBuilding = false;
            if (building.Type == BuildingEnums.Site)
            {
                isNewBuilding = true;
                var tab = task.Building.GetBuildingsCategory();

                var result = _generalHelper.SwitchTab(accountId, tab);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else
            {
                if (building.Level == -1)
                {
                    isNewBuilding = true;
                }
                else
                {
                    if (task.Building.HasMultipleTabs() && building.Level != 0)
                    {
                        var result = _generalHelper.SwitchTab(accountId, 0);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
            }

            return isNewBuilding;
        }

        public Result Construct(int accountId, PlanTask task)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var node = html.GetElementbyId($"contract_building{(int)task.Building}");
            if (node is null)
            {
                return Result.Fail(new Retry($"Cannot find building box for {task.Building}"));
            }
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button is null)
            {
                return Result.Fail(new Retry($"Cannot find Build button for {task.Building}"));
            }
            var result = _generalHelper.Click(accountId, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result Upgrade(int accountId, PlanTask task)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            if (container is null)
            {
                return Result.Fail(new Retry($"Cannot find upgrading box for {task.Building}"));
            }
            var upgradeButton = container.Descendants("button").FirstOrDefault(x => x.HasClass("build"));

            if (upgradeButton == null)
            {
                return Result.Fail(new Retry($"Cannot find upgrade button for {task.Building}"));
            }

            var result = _generalHelper.Click(accountId, By.XPath(upgradeButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result UpgradeAds_UpgradeClicking(int accountId, PlanTask task)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var nodeFastUpgrade = html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("videoFeatureButton") && x.HasClass("green"));
            if (nodeFastUpgrade is null)
            {
                return Result.Fail(new Retry($"Cannot find fast upgrade button for {task.Building}"));
            }

            var result = _generalHelper.Click(accountId, By.XPath(nodeFastUpgrade.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result UpgradeAds_AccpetAds(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var nodeNotShowAgainConfirm = html.DocumentNode.SelectSingleNode("//input[@name='adSalesVideoInfoScreen']");
            if (nodeNotShowAgainConfirm is not null)
            {
                var result = _generalHelper.Click(accountId, By.XPath(nodeNotShowAgainConfirm.ParentNode.XPath));
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                chromeBrowser.GetChrome().ExecuteScript("jQuery(window).trigger('showVideoWindowAfterInfoScreen')");
            }
            return Result.Ok();
        }

        public void UpgradeAds_CloseOtherTab(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.GetChrome();
            var current = chrome.CurrentWindowHandle;
            while (chrome.WindowHandles.Count > 1)
            {
                var others = chrome.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                chrome.SwitchTo().Window(others);
                chrome.Close();
                chrome.SwitchTo().Window(current);
            }
        }

        public Result UpgradeAds_ClickPlayAds(int accountId, PlanTask task)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var nodeIframe = html.GetElementbyId("videoFeature");
            if (nodeIframe is null)
            {
                return Result.Fail(new Retry($"Cannot find iframe for {task.Building}"));
            }

            var result = _generalHelper.Click(accountId, By.XPath(nodeIframe.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var chrome = chromeBrowser.GetChrome();
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

                result = _generalHelper.Click(accountId, By.XPath(nodeIframe.XPath), waitPageLoaded: false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                chrome.SwitchTo().DefaultContent();
            }
            while (true);
            return Result.Ok();
        }

        public Result UpgradeAds_DontShowThis(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            if (html.GetElementbyId("dontShowThisAgain") is not null)
            {
                var result = _generalHelper.Click(accountId, By.Id("dontShowThisAgain"));
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                result = _generalHelper.Click(accountId, By.ClassName("dialogButtonOk"));
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result UpgradeAds(int accountId, PlanTask task)
        {
            var result = UpgradeAds_UpgradeClicking(accountId, task);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            Thread.Sleep(Random.Shared.Next(2400, 5300));

            result = UpgradeAds_AccpetAds(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            Thread.Sleep(Random.Shared.Next(20000, 25000));

            UpgradeAds_CloseOtherTab(accountId);

            result = UpgradeAds_ClickPlayAds(accountId, task);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.WaitPageChanged(accountId, "dorf");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.WaitPageLoaded(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            Thread.Sleep(1000);

            result = UpgradeAds_DontShowThis(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result<PlanTask> ChooseBuilding(int accountId, int villageId)
        {
            PlanTask chosenTask = null;
            var tasks = _planManager.GetList(villageId, true);
            if (tasks.Count == 0)
            {
                return Result.Fail(new Skip("Queue is empty."));
            }

            using var context = _contextFactory.CreateDbContext();
            var currentList = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0).ToList();
            var totalBuild = currentList.Count;

            if (totalBuild == 0)
            {
                chosenTask = GetFirstTask(villageId);
                if (chosenTask.Type == PlanTypeEnums.General && chosenTask.Building != BuildingEnums.Cropland)
                {
                    var freeCrop = context.VillagesResources.Find(villageId).FreeCrop;
                    if (freeCrop < 4)
                    {
                        return Result.Fail(new LackFreeCrop());
                    }
                }
                return Result.Ok(chosenTask);
            }

            var accountInfo = context.AccountsInfo.Find(accountId);
            var tribe = accountInfo.Tribe;
            var hasPlusAccount = accountInfo.HasPlusAccount;
            var setting = context.VillagesSettings.Find(villageId);

            var romanAdvantage = tribe == TribeEnums.Romans && !setting.IsIgnoreRomanAdvantage;

            var maxBuild = 1;
            if (hasPlusAccount) maxBuild++;
            if (romanAdvantage) maxBuild++;

            if (totalBuild == maxBuild)
            {
                return Result.Fail(BuildingQueue.Full);
            }

            // non-Roman tribe can build anything if there is atleast 1 free slot
            if (tribe != TribeEnums.Romans && maxBuild - totalBuild >= 1)
            {
                chosenTask = GetFirstTask(villageId);
                if (chosenTask.Type == PlanTypeEnums.General && chosenTask.Building != BuildingEnums.Cropland)
                {
                    var freeCrop = context.VillagesResources.Find(villageId).FreeCrop;
                    if (freeCrop < 4)
                    {
                        return Result.Fail(new LackFreeCrop());
                    }
                }
                return Result.Ok(chosenTask);
            }
            // Roman tribe can build anything if there is atleast 2 free slots
            if (tribe == TribeEnums.Romans && maxBuild - totalBuild >= 2)
            {
                chosenTask = GetFirstTask(villageId);
                if (chosenTask.Type == PlanTypeEnums.General && chosenTask.Building != BuildingEnums.Cropland)
                {
                    var freeCrop = context.VillagesResources.Find(villageId).FreeCrop;
                    if (freeCrop < 4)
                    {
                        return Result.Fail(new LackFreeCrop());
                    }
                }
                return Result.Ok(chosenTask);
            }

            var numQueueRes = tasks.Count(x => x.Building.IsResourceField() || x.Type == PlanTypeEnums.ResFields);
            var numQueueBuilding = tasks.Count - numQueueRes;

            var numCurrentRes = currentList.Count(x => x.Type.IsResourceField());
            var numCurrentBuilding = totalBuild - numCurrentRes;

            if (numCurrentRes > numCurrentBuilding)
            {
                var freeCrop = context.VillagesResources.Find(villageId).FreeCrop;
                if (freeCrop < 4)
                {
                    return Result.Fail(BuildingQueue.LackFreeCrop);
                }

                if (numQueueBuilding == 0)
                {
                    return Result.Fail(BuildingQueue.NoBuilding);
                }

                chosenTask = GetFirstBuildingTask(villageId);
                return Result.Ok(chosenTask);
            }

            if (numCurrentBuilding > numCurrentRes)
            {
                // no need check free crop, there is magic make sure this always choose crop
                // jk, because of how we check free crop later, first res task is always crop
                if (numQueueRes == 0)
                {
                    return Result.Fail(BuildingQueue.NoResource);
                }

                chosenTask = GetFirstResTask(villageId);
                return Result.Ok(chosenTask);
            }

            // if same means 1 R and 1 I already, 1 ANY will be choose below
            chosenTask = GetFirstTask(villageId);
            return Result.Ok(chosenTask);
        }
    }
}