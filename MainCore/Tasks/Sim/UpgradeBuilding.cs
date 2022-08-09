using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.TravianData;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainCore.Tasks.Sim
{
    public class UpgradeBuilding : BotTask
    {
        public UpgradeBuilding(int villageId, int accountId) : base(accountId)
        {
            _villageId = villageId;
        }

        public override string Name => $"Upgrade building village {VillageId}";

        private readonly int _villageId;
        public int VillageId => _villageId;

        public override void Execute()
        {
            using var context = ContextFactory.CreateDbContext();

            do
            {
                var buildingTask = UpgradeBuildingHelper.NextBuildingTask(context, PlanManager, LogManager, AccountId, VillageId);
                if (buildingTask is null)
                {
                    var tasks = PlanManager.GetList(VillageId);
                    if (tasks.Count == 0)
                    {
                        LogManager.Information(AccountId, "Queue is empty.");
                        return;
                    }

                    NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
                    NavigateHelper.GoRandomDorf(ChromeBrowser);
                    UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, VillageId);

                    var firstComplete = context.VillagesCurrentlyBuildings.Find(VillageId, 0);
                    if (firstComplete.CompleteTime == DateTime.MaxValue)
                    {
                        continue;
                    }

                    ExecuteAt = firstComplete.CompleteTime.AddSeconds(10);
                    LogManager.Information(AccountId, $"Next building will be contructed after {firstComplete.Type} - level {firstComplete.Level} complete. ({ExecuteAt})");
                    return;
                }

                if (buildingTask.Type == PlanTypeEnums.ResFields)
                {
                    var task = UpgradeBuildingHelper.ExtractResField(context, VillageId, buildingTask);
                    if (task is null)
                    {
                        PlanManager.Remove(VillageId, task);
                        continue;
                    }
                    else
                    {
                        PlanManager.Insert(VillageId, 0, task);
                        continue;
                    }
                }

                NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
                UpdateHelper.UpdateResource(context, ChromeBrowser, VillageId);

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
                    PlanManager.Insert(VillageId, 0, task);
                    continue;
                }

                // move to correct page
                var dorf = BuildingsHelper.GetDorf(buildingTask.Location);
                switch (dorf)
                {
                    case 1:
                        NavigateHelper.ToDorf1(ChromeBrowser, true);
                        UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, VillageId);
                        UpdateHelper.UpdateDorf1(context, ChromeBrowser, VillageId);
                        break;

                    case 2:
                        NavigateHelper.ToDorf2(ChromeBrowser, true);
                        UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, VillageId);
                        UpdateHelper.UpdateDorf2(context, ChromeBrowser, AccountId, VillageId);
                        break;
                }

                var building = context.VillagesBuildings.Find(VillageId, buildingTask.Location);
                if (building.Level >= buildingTask.Level)
                {
                    PlanManager.Remove(VillageId, buildingTask);
                    continue;
                }
                var currently = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Location == buildingTask.Location);
                if (currently is not null && currently.Level >= buildingTask.Level)
                {
                    PlanManager.Remove(VillageId, buildingTask);
                    continue;
                }

                NavigateHelper.GoToBuilding(ChromeBrowser, buildingTask.Location);

                bool isNewBuilding = false;
                if (building.Type == BuildingEnums.Site)
                {
                    isNewBuilding = true;
                    var tab = BuildingsData.GetBuildingsCategory(buildingTask.Building);
                    NavigateHelper.SwitchTab(ChromeBrowser, tab);
                }
                else
                {
                    if (BuildingsData.HasMultipleTabs(buildingTask.Building))
                    {
                        NavigateHelper.SwitchTab(ChromeBrowser, 0);
                    }
                }

                var resNeed = CheckHelper.GetResourceNeed(ChromeBrowser, buildingTask.Building, isNewBuilding);
                var resCurrent = context.VillagesResources.Find(VillageId);
                if (resNeed[0] > resCurrent.Wood || resNeed[1] > resCurrent.Clay || resNeed[2] > resCurrent.Iron || resNeed[3] > resCurrent.Crop)
                {
                    LogManager.Information(AccountId, "Don't have enough resources.");
                    break;
                }

                if (isNewBuilding) Construct(buildingTask);
                else Upgrade(buildingTask);
            }
            while (true);
        }

        private void Construct(PlanTask buildingTask)
        {
            var html = ChromeBrowser.GetHtml();
            var node = html.GetElementbyId($"contract_building{(int)buildingTask.Building}");
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button == null)
            {
                throw new Exception($"Cannot find Build button for {buildingTask.Building}");
            }

            var chrome = ChromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(button.XPath));
            if (elements.Count == 0)
            {
                throw new Exception($"Cannot find Build button for {buildingTask.Building}");
            }
            elements[0].Click();

            if (buildingTask.Level == 1)
            {
                PlanManager.Remove(VillageId, buildingTask);
            }
        }

        private void Upgrade(PlanTask buildingTask)
        {
            var html = ChromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            var upgradeButton = container.Descendants("button").FirstOrDefault(x => x.HasClass("build"));

            if (upgradeButton == null)
            {
                throw new Exception("Cannot find upgrade button");
            }

            var chrome = ChromeBrowser.GetChrome();

            var elements = chrome.FindElements(By.XPath(upgradeButton.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find upgrade button");
            }

            elements[0].Click();
        }
    }
}