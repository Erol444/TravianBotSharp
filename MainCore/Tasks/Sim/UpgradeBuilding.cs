using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.Tasks.Update;
using OpenQA.Selenium;
using System;
using System.Linq;
using MainCore.Tasks.Misc;

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

using System.Collections.Generic;
using System.Threading;

#elif TTWARS

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Tasks.Sim
{
    public class UpgradeBuilding : VillageBotTask
    {
        public UpgradeBuilding(int villageId, int accountId) : base(villageId, accountId, "Upgrade building")
        {
        }

        public override void Execute()
        {
            do
            {
                StopFlag = false;
                if (Cts.IsCancellationRequested) return;

                var buildingTask = SelectBuilding();
                if (Cts.IsCancellationRequested) return;
                if (StopFlag) return;
                if (buildingTask is null) continue;

                if (IsAutoBuilding(buildingTask)) continue;
                if (Cts.IsCancellationRequested) return;

                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
                    if (Cts.IsCancellationRequested) return;
                    UpdateHelper.UpdateResource(context, _chromeBrowser, VillageId);
                    if (Cts.IsCancellationRequested) return;
                }

                if (!IsEnoughFreeCrop(buildingTask)) continue;
                if (Cts.IsCancellationRequested) return;

                if (IsThereCompleteBuilding(buildingTask)) continue;
                if (Cts.IsCancellationRequested) return;

                var isNewBuilding = GotoBuilding(buildingTask);
                if (Cts.IsCancellationRequested) return;

                var isEnoughResource = IsEnoughResource(buildingTask, isNewBuilding);
                if (StopFlag) return;
                if (!isEnoughResource) continue;

                if (Cts.IsCancellationRequested) return;
                if (isNewBuilding) Construct(buildingTask);
                else
                {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                    using var context = _contextFactory.CreateDbContext();
                    if (CheckHelper.IsNeedAdsUpgrade(_chromeBrowser, context, VillageId, buildingTask))
                    {
                        UpgradeAds(buildingTask);
                    }
                    else
                    {
                        Upgrade(buildingTask);
                    }

#elif TTWARS
                    Upgrade(buildingTask);
#else

#error You forgot to define Travian version here

#endif
                }

                Update();
            }
            while (true);
        }

        private void Construct(PlanTask buildingTask)
        {
            var html = _chromeBrowser.GetHtml();
            var node = html.GetElementbyId($"contract_building{(int)buildingTask.Building}");
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button == null)
            {
                throw new Exception($"Cannot find Build button for {buildingTask.Building}");
            }

            var chrome = _chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(button.XPath));
            if (elements.Count == 0)
            {
                throw new Exception($"Cannot find Build button for {buildingTask.Building}");
            }
            elements[0].Click();

            if (buildingTask.Level == 1)
            {
                _planManager.Remove(VillageId, buildingTask);
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
            }
        }

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

        private void UpgradeAds(PlanTask buildingTask)
        {
            var html = _chromeBrowser.GetHtml();
            var chrome = _chromeBrowser.GetChrome();

            {
                var nodeFastUpgrade = html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("videoFeatureButton") && x.HasClass("green"));
                if (nodeFastUpgrade is null)
                {
                    throw new Exception($"Cannot find fast upgrade button for {buildingTask.Building}");
                }
                var elements = chrome.FindElements(By.XPath(nodeFastUpgrade.XPath));
                if (elements.Count == 0)
                {
                    throw new Exception($"Cannot find fast upgrade button for {buildingTask.Building}");
                }
                elements[0].Click();
            }
            var rand = new Random(DateTime.Now.Second);

            Thread.Sleep(rand.Next(2400, 5300));
            html = _chromeBrowser.GetHtml();
            {
                var nodeNotShowAgainConfirm = html.DocumentNode.SelectSingleNode("//input[@name='adSalesVideoInfoScreen']");
                if (nodeNotShowAgainConfirm is not null)
                {
                    var elements = chrome.FindElements(By.XPath(nodeNotShowAgainConfirm.ParentNode.XPath));
                    elements[0].Click();
                    chrome.ExecuteScript("jQuery(window).trigger('showVideoWindowAfterInfoScreen')");
                }
            }
            {
                var current = chrome.CurrentWindowHandle;
                while (chrome.WindowHandles.Count > 1)
                {
                    if (Cts.IsCancellationRequested) return;
                    var other = chrome.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                    chrome.SwitchTo().Window(other);
                    chrome.Close();
                    chrome.SwitchTo().Window(current);
                }
            }
            Thread.Sleep(rand.Next(20000, 25000));
            html = _chromeBrowser.GetHtml();
            {
                var nodeIframe = html.GetElementbyId("videoFeature");
                if (nodeIframe is null)
                {
                    throw new Exception($"Cannot find iframe for {buildingTask.Building}");
                }
                var elementsIframe = chrome.FindElements(By.XPath(nodeIframe.XPath));
                if (elementsIframe.Count == 0)
                {
                    throw new Exception($"Cannot find iframe for {buildingTask.Building}");
                }
                elementsIframe[0].Click();
                chrome.SwitchTo().DefaultContent();

                Thread.Sleep(rand.Next(1300, 2000));

                do
                {
                    if (Cts.IsCancellationRequested) return;

                    var handles = chrome.WindowHandles;
                    if (handles.Count == 1) break;

                    var current = chrome.CurrentWindowHandle;
                    var other = chrome.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                    chrome.SwitchTo().Window(other);
                    chrome.Close();
                    chrome.SwitchTo().Window(current);
                    elementsIframe[0].Click();
                    chrome.SwitchTo().DefaultContent();
                }
                while (true);
            }

            {
                var wait = _chromeBrowser.GetWait();
                wait.Until(driver => driver.Url.Contains("dorf"));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                html = _chromeBrowser.GetHtml();
                if (html.GetElementbyId("dontShowThisAgain") is not null)
                {
                    var dontshowthisagain = chrome.FindElements(By.Id("dontShowThisAgain"));
                    dontshowthisagain[0].Click();
                    Thread.Sleep(800);
                    var dialogbuttonok = chrome.FindElements(By.ClassName("dialogButtonOk"));
                    dialogbuttonok[0].Click();
                }
            }
        }

#elif TTWARS

#else

#error You forgot to define Travian version here

#endif

        private void Upgrade(PlanTask buildingTask)
        {
            var html = _chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            var upgradeButton = container.Descendants("button").FirstOrDefault(x => x.HasClass("build"));

            if (upgradeButton == null)
            {
                throw new Exception($"Cannot find upgrade button for {buildingTask.Building}");
            }

            var chrome = _chromeBrowser.GetChrome();

            var elements = chrome.FindElements(By.XPath(upgradeButton.XPath));
            if (elements.Count == 0)
            {
                throw new Exception($"Cannot find upgrade button for {buildingTask.Building}");
            }

            elements[0].Click();
        }

        private void Update()
        {
            {
                NavigateHelper.WaitPageLoaded(_chromeBrowser);
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            var taskUpdateVillage = new UpdateVillage(VillageId, AccountId);
            taskUpdateVillage.CopyFrom(this);
            taskUpdateVillage.Execute();
        }

        private PlanTask SelectBuilding()
        {
            using var context = _contextFactory.CreateDbContext();

            UpgradeBuildingHelper.RemoveFinishedCB(context, VillageId);
            var buildingTask = UpgradeBuildingHelper.NextBuildingTask(context, _planManager, _logManager, AccountId, VillageId);
            if (buildingTask is null)
            {
                var tasks = _planManager.GetList(VillageId);
                if (tasks.Count == 0)
                {
                    _logManager.Information(AccountId, "Queue is empty.");
                    StopFlag = true;
                    return null;
                }
                if (!_chromeBrowser.GetCurrentUrl().Contains("dorf"))
                {
                    NavigateHelper.GoRandomDorf(_chromeBrowser, context, AccountId);
                }
#if TTWARS
                Refresh();
#elif TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

#else

#error You forgot to define Travian version here

#endif
                var updateTask = new UpdateVillage(VillageId, AccountId);
                updateTask.CopyFrom(this);
                updateTask.Execute();

                var firstComplete = context.VillagesCurrentlyBuildings.Find(VillageId, 0);
                if (firstComplete.Level == -1)
                {
                    return null;
                }
#if TTWARS
                ExecuteAt = firstComplete.CompleteTime.AddSeconds(1);
#elif TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                ExecuteAt = firstComplete.CompleteTime.AddSeconds(10);
#else

#error You forgot to define Travian version here

#endif
                _logManager.Information(AccountId, $"Next building will be contructed after {firstComplete.Type} - level {firstComplete.Level} complete. ({ExecuteAt})");
                StopFlag = true;
                return null;
            }

            if (buildingTask.Type == PlanTypeEnums.General)
            {
                _logManager.Information(AccountId, $"Next building will be contructed: {buildingTask.Building} - level {buildingTask.Level}.");
            }
            else if (buildingTask.Type == PlanTypeEnums.ResFields)
            {
                _logManager.Information(AccountId, $"Next building will be contructed: {buildingTask.ResourceType} - {buildingTask.BuildingStrategy} - level {buildingTask.Level}.");
            }
            return buildingTask;
        }

        private bool IsAutoBuilding(PlanTask buildingTask)
        {
            if (buildingTask.Type == PlanTypeEnums.ResFields)
            {
                using var context = _contextFactory.CreateDbContext();
                var task = UpgradeBuildingHelper.ExtractResField(context, VillageId, buildingTask);
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

        private bool IsEnoughFreeCrop(PlanTask buildingTask)
        {
            using var context = _contextFactory.CreateDbContext();
            if (context.VillagesResources.Find(VillageId).FreeCrop <= 5 && buildingTask.Building != BuildingEnums.Cropland)
            {
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
            return true;
        }

        private bool IsThereCompleteBuilding(PlanTask buildingTask)
        {
            using var context = _contextFactory.CreateDbContext();
            // move to correct page
            var dorf = BuildingsHelper.GetDorf(buildingTask.Location);
            var url = _chromeBrowser.GetCurrentUrl();
            switch (dorf)
            {
                case 1:
                    {
                        var taskUpdate = new UpdateDorf1(VillageId, AccountId);
                        taskUpdate.CopyFrom(this);
                        taskUpdate.Execute();
                    }
                    break;

                case 2:
                    {
                        var taskUpdate = new UpdateDorf2(VillageId, AccountId);
                        taskUpdate.CopyFrom(this);
                        taskUpdate.Execute();
                    }
                    break;
            }

            var building = context.VillagesBuildings.Find(VillageId, buildingTask.Location);
            if (building.Level >= buildingTask.Level)
            {
                _planManager.Remove(VillageId, buildingTask);
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
                return true;
            }
            var currently = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Location == buildingTask.Location);
            if (currently is not null && currently.Level >= buildingTask.Level)
            {
                _planManager.Remove(VillageId, buildingTask);
                _eventManager.OnVillageBuildQueueUpdate(VillageId);
                return true;
            }
            return false;
        }

        private bool GotoBuilding(PlanTask buildingTask)
        {
            using var context = _contextFactory.CreateDbContext();

            NavigateHelper.GoToBuilding(_chromeBrowser, buildingTask.Location, context, AccountId);

            var building = context.VillagesBuildings.Find(VillageId, buildingTask.Location);

            bool isNewBuilding = false;
            if (building.Type == BuildingEnums.Site)
            {
                isNewBuilding = true;
                var tab = buildingTask.Building.GetBuildingsCategory();
                NavigateHelper.SwitchTab(_chromeBrowser, tab, context, AccountId);
            }
            else
            {
                if (building.Level == -1)
                {
                    isNewBuilding = true;
                }
                else
                {
                    if (buildingTask.Building.HasMultipleTabs())
                    {
                        NavigateHelper.SwitchTab(_chromeBrowser, 0, context, AccountId);
                    }
                }
            }
            return isNewBuilding;
        }

        private bool IsEnoughResource(PlanTask buildingTask, bool isNewBuilding)
        {
            var resNeed = CheckHelper.GetResourceNeed(_chromeBrowser, buildingTask.Building, isNewBuilding);
            using var context = _contextFactory.CreateDbContext();
            var resCurrent = context.VillagesResources.Find(VillageId);
            if (resNeed[0] > resCurrent.Wood || resNeed[1] > resCurrent.Clay || resNeed[2] > resCurrent.Iron || resNeed[3] > resCurrent.Crop)
            {
                var resMissing = new long[] { resNeed[0] - resCurrent.Wood, resNeed[1] - resCurrent.Clay, resNeed[2] - resCurrent.Iron, resNeed[3] - resCurrent.Crop };
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

                var setting = context.VillagesSettings.Find(VillageId);
                if (!setting.IsUseHeroRes)
                {
                    _logManager.Information(AccountId, "Don't have enough resources.");
                    var production = context.VillagesProduction.Find(VillageId);
                    var timeEnough = production.GetTimeWhenEnough(resMissing);
                    ExecuteAt = timeEnough;
                    StopFlag = true;
                    return false;
                }
                var taskUpdate = new UpdateHeroItems(AccountId);
                taskUpdate.CopyFrom(this);
                taskUpdate.Execute();
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
                    StopFlag = true;
                    return false;
                }

                var items = new List<(HeroItemEnums, int)>()
                    {
                        (HeroItemEnums.Wood, (int)resMissing[0]),
                        (HeroItemEnums.Clay, (int)resMissing[1]),
                        (HeroItemEnums.Iron, (int)resMissing[2]),
                        (HeroItemEnums.Crop, (int)resMissing[3]),
                    };
                var taskEquip = new UseHeroResources(VillageId, AccountId, items);
                taskEquip.CopyFrom(this);
                taskEquip.Execute();
                if (IsThereCompleteBuilding(buildingTask)) return false;
                GotoBuilding(buildingTask);
#elif TTWARS
                _logManager.Information(AccountId, "Don't have enough resources.");
                var production = context.VillagesProduction.Find(VillageId);
                var timeEnough = production.GetTimeWhenEnough(resMissing);
                ExecuteAt = timeEnough;
                StopFlag = true;
                return false;
#else

#error You forgot to define Travian version here

#endif
            }
            return true;
        }
    }
}