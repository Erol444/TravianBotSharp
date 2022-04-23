using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;
using TbsCore.TravianData;

using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    public class UpgradeBuilding : BotTask
    {
        private BuildingTask _buildingTask;
        private readonly Random rand = new Random();
        private bool construct = false;
        private bool result = true;

        public override async Task<TaskRes> Execute(Account acc)
        {
            StopFlag = false;
            do
            {
                if (StopFlag) return TaskRes.Executed;

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Choose building to build...", this);
                    _buildingTask = await ChooseTask(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (_buildingTask == null) continue;
                    if (_buildingTask.TaskType == BuildingType.AutoUpgradeResFields)
                    {
                        acc.Logger.Information($"{_buildingTask.ResourceType} - Level {_buildingTask.Level} is chosen", this);
                    }
                    else
                    {
                        acc.Logger.Information($"{_buildingTask.Building} - Level {_buildingTask.Level} is chosen", this);
                    }
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Check condition ...", this);
                    var result = TaskTypeCondition(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Check Free crop ...", this);
                    result = await FreeCropCondition(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                // Fast building for TTWars
                if (acc.AccInfo.ServerVersion == ServerVersionEnum.TTwars && !_buildingTask.ConstructNew)
                {
                    acc.Logger.Information("Try using TTWars fast build method", this);
                    await Task.Delay(AccountHelper.Delay(acc));
                    var fastUpgrade = await TTWarsTryFastUpgrade(acc, $"{acc.AccInfo.ServerUrl}/build.php?id={_buildingTask.BuildingId}");
                    if (fastUpgrade) continue;
                    acc.Logger.Information("Using TTWars fast build method failed. Continue normal method", this);
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Moving to building ...", this);
                    await MoveIntoBuilding(acc);
                    if (StopFlag) return TaskRes.Executed;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Check correct page ...", this);
                    var result = IsContructPage(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }
                {
                    acc.Logger.Information("Check enough resource ...", this);
                    var result = await IsEnoughRes(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }
                {
                    bool result;
                    if (construct)
                    {
                        acc.Logger.Information("Starting construct ...", this);
                        result = await Construct(acc);
                    }
                    else
                    {
                        acc.Logger.Information("Starting upgrade ...", this);
                        result = await Upgrade(acc);
                    }
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }
                await DriverHelper.WaitPageChange(acc, "dorf");
            }
            while (true);
        }

        /// <summary>
        /// Building isn't constructed yet - construct it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskResult</returnss>
        private async Task<bool> Construct(Account acc)
        {
            acc.Wb.UpdateHtml();
            var node = acc.Wb.Html.GetElementbyId($"contract_building{(int)_buildingTask.Building}");
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button == null)
            {
                Retry(acc, "Button disappear.");
                return false;
            }

            await AccountHelper.DelayWait(acc);
            var element = acc.Wb.Driver.FindElement(By.XPath(button.XPath));
            if (element == null)
            {
                Retry(acc, "Button disappear.");
                return false;
            }
            element.Click();
            _buildingTask.ConstructNew = false;

            if (_buildingTask.Level == 1)
            {
                RemoveCurrentTask();
            }

            return true;
        }

        /// <summary>
        /// Building is already constructed, upgrade it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskResult</returns>
        private async Task<bool> Upgrade(Account acc)
        {
            acc.Wb.UpdateHtml();
            HtmlNode node = acc.Wb.Html.GetElementbyId("build");
            (var buildingEnum, var lvl) = InfrastructureParser.UpgradeBuildingGetInfo(node);

            if (buildingEnum == BuildingEnum.Site || lvl == -1)
            {
                acc.Logger.Warning($"Can't upgrade building {_buildingTask.Building} in village {Vill.Name}. Will be removed from the queue.");
                RemoveCurrentTask();
                return false;
            }

            // Basic task already on/above desired level, don't upgrade further
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Id == this._buildingTask.BuildingId);
            lvl = building.Level;
            // Check if building is under construction
            if (building.UnderConstruction)
            {
                // Check currently building
                var cb = Vill.Build.CurrentlyBuilding.OrderByDescending(x => x.Level).FirstOrDefault(x => x.Location == building.Id);
                if (cb != null && lvl < cb.Level) lvl = cb.Level;
            }

            if (lvl >= _buildingTask.Level)
            {
                acc.Logger.Information($"{_buildingTask.Building} is already level {lvl} in village {Vill.Name}. Will be removed from the queue.");
                RemoveCurrentTask();
                return false;
            }

            var container = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            var buttons = container?.Descendants("button");
            if (buttons == null)
            {
                acc.Logger.Information($"We wanted to upgrade {_buildingTask.Building}, but no 'upgrade' button was found! Url={acc.Wb.CurrentUrl}", this);
                return false;
            }

            var errorMessage = acc.Wb.Html.GetElementbyId("build")
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("upgradeBuilding"))?
                .Descendants("div")?
                .FirstOrDefault(x => x.HasClass("errorMessage"));
            HtmlNode upgradeButton = buttons.FirstOrDefault(x => x.HasClass("build"));

            if (upgradeButton == null)
            {
                Retry(acc, $"We wanted to upgrade {_buildingTask.Building}, but no 'upgrade' button was found!");
                return false;
            }

            // Not enough resources?
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5 && errorMessage != null)
            {
                Retry(acc, $"We wanted to upgrade {_buildingTask.Building}, but there was an error message:\n{errorMessage.InnerText}");
                return false;
            }

            var buildDuration = InfrastructureParser.GetBuildDuration(container, acc.AccInfo.ServerVersion);

            acc.Logger.Information("Complete checking");
            acc.Logger.Information($"Upgrading {_buildingTask.Building} to level {lvl + 1} in {Vill.Name}");

            var watchAd = false;
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5 && buildDuration.TotalMinutes > acc.Settings.WatchAdAbove)
            {
                acc.Logger.Information("Try using watch ads upgrade button");
                watchAd = await TryFastUpgrade(acc);
            }

            if (!watchAd)
            {
                await AccountHelper.DelayWait(acc);
                acc.Logger.Information("Using normal upgrade button");

                upgradeButton = buttons.FirstOrDefault(x => x.HasClass("build"));

                var element = acc.Wb.Driver.FindElement(By.XPath(upgradeButton.XPath));
                if (element == null)
                {
                    Retry(acc, "Button disappear.");
                    return false;
                }
                element.Click();
            }

            acc.Logger.Information($"Upgraded {_buildingTask.Building} to level {lvl + 1} in {Vill.Name}");
            if (_buildingTask.Level == lvl + 1)
            {
                RemoveCurrentTask();
            }
            return true;
        }

        private void RemoveCurrentTask() => Vill.Build.Tasks.Remove(this._buildingTask);

        private void PostTaskCheckDorf(Account acc)
        {
            // Check if residence is getting upgraded to level 10 => train settlers
            var cbResidence = Vill.Build
                .CurrentlyBuilding
                .FirstOrDefault(x => (x.Building == BuildingEnum.Residence ||
                                      x.Building == BuildingEnum.Palace ||
                                      x.Building == BuildingEnum.CommandCenter) &&
                                      x.Level == 10);
            if (cbResidence != null &&
                acc.NewVillages.AutoSettleNewVillages &&
                Vill.Troops.Settlers == 0)
            {
                acc.Tasks.Add(
                    new TrainSettlers()
                    {
                        ExecuteAt = cbResidence.Duration.AddSeconds(5),
                        Vill = Vill,
                        // For high speed servers, you want to train settlers asap
                        Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
                    }, true, Vill);
            }

            // Check if the task is completed
            var taskCb = Vill.Build
                .CurrentlyBuilding
                .OrderByDescending(x => x.Level)
                .FirstOrDefault(x => x.Location == this._buildingTask.BuildingId);
            if (taskCb != null && this._buildingTask.TaskType == BuildingType.General && this._buildingTask.Level <= taskCb.Level) RemoveCurrentTask();
        }

        /// <summary>
        /// Tries to watch an Ad for +25% faster upgrade
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>Whether bot watched the ad</returns>
        private async Task<bool> TryFastUpgrade(Account acc)
        {
            var nodeFastUpgrade = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("videoFeatureButton") && x.HasClass("green"));
            if (nodeFastUpgrade == null) return false;
            var href = nodeFastUpgrade.GetAttributeValue("onclick", "");
            var script = href.Replace("&amp;", "&");
            acc.Wb.Driver.ExecuteScript(script);
            await Task.Delay(rand.Next(900, 1300));
            // Confirm

            {
                var result = await Update(acc);
                if (!result) return false;
            }

            var nodeNotShowAgainConfirm = acc.Wb.Html.DocumentNode.SelectSingleNode("//input[@name='adSalesVideoInfoScreen']");
            if (nodeNotShowAgainConfirm != null)
            {
                acc.Logger.Information("Detected Watch AD diaglog. Choose Don't show again. ...");
                var element = acc.Wb.Driver.FindElement(By.XPath(nodeNotShowAgainConfirm.XPath));
                Actions act = new Actions(acc.Wb.Driver);
                act.MoveToElement(element).Click().Build().Perform();
                acc.Wb.Driver.ExecuteScript("jQuery(window).trigger('showVideoWindowAfterInfoScreen')");
            }

            // click to play video
            acc.Logger.Information("Waiting ads video play button show");

            {
                var result = await Update(acc);
                if (!result) return false;
            }
            var nodeIframe = acc.Wb.Html.GetElementbyId("videoFeature");
            if (nodeIframe == null)
            {
                return false;
            }

            {
                await Task.Delay(rand.Next(20000, 30000));

                var elementIframe = acc.Wb.Driver.FindElement(By.XPath(nodeIframe.XPath));
                Actions act = new Actions(acc.Wb.Driver);
                var action = act.MoveToElement(elementIframe).Click().Build();
                action.Perform();

                await Task.Delay(rand.Next(10000, 15000));

                do
                {
                    var handles = acc.Wb.Driver.WindowHandles;
                    if (handles.Count == 1) break;

                    acc.Logger.Information("Detect auto play ads, bot maybe pause ads. Great work Travian Devs");
                    var current = acc.Wb.Driver.CurrentWindowHandle;
                    var other = acc.Wb.Driver.WindowHandles.FirstOrDefault(x => !x.Equals(current));
                    acc.Wb.Driver.SwitchTo().Window(other);
                    acc.Wb.Driver.Close();
                    acc.Wb.Driver.SwitchTo().Window(current);
                    action.Perform();
                }
                while (true);
            }

            acc.Wb.Driver.SwitchTo().DefaultContent();

            acc.Logger.Information("Clicked play button, if ads doesn't play please click to help bot");
            acc.Logger.Information("Cooldown 3 mins. If building cannot upgrade will use normal button");

            {
                var result = await DriverHelper.WaitPageChange(acc, "dorf", 3);
                if (!result)
                {
                    acc.Wb.UpdateHtml();
                    if (acc.Wb.Html.GetElementbyId("dontShowThisAgain") != null)
                    {
                        await DriverHelper.ClickById(acc, "dontShowThisAgain");
                        await Task.Delay(800);
                        await DriverHelper.ClickByClassName(acc, "dialogButtonOk ok");
                        return true;
                    }
                    else
                    {
                        await acc.Wb.Refresh();
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> TTWarsTryFastUpgrade(Account acc, string url)
        {
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Id == _buildingTask.BuildingId);
            var lvl = building.Level;
            if (building.UnderConstruction) lvl++;

            var neededRes = BuildingsData.GetBuildingCost(building.Type, lvl + 1);

            if (ResourcesHelper.IsEnoughRes(Vill, neededRes) &&
                lvl != 0 &&
                lvl < _buildingTask.Level)
            {
                await acc.Wb.Navigate(url + "&fastUP=1");
                acc.Logger.Information($"Started (fast) upgrading {building.Type} to level {lvl} in {this.Vill?.Name}");

                var build = acc.Wb.Html.GetElementbyId("build");
                if (build != null) RemoveCurrentTask(); // Already on max lvl
                else PostTaskCheckDorf(acc);
                return true;
            }

            return false;
        }

        private async Task GoRandomDorf(Account acc)
        {
            var chanceDorf2 = rand.Next(1, 100);
            if (chanceDorf2 >= 50)
            {
                await NavigationHelper.ToDorf2(acc);
            }
            else
            {
                await NavigationHelper.ToDorf1(acc);
            }
        }

        private async Task<BuildingTask> ChooseTask(Account acc)
        {
            var nextTask = UpgradeBuildingHelper.NextBuildingTask(acc, Vill);

            if (nextTask == null)
            {
                if (Vill.Build.Tasks.Count == 0)
                {
                    StopFlag = true;
                    return nextTask;
                }

                acc.Logger.Information("Cannot choose next building task.");
                acc.Logger.Information("Checking current village ...");
                await NavigationHelper.SwitchVillage(acc, Vill);
                acc.Logger.Information("Update currently building ... ");
                await GoRandomDorf(acc);

                var firstComplete = Vill.Build.CurrentlyBuilding.FirstOrDefault();
                if (firstComplete == null)
                {
                    Retry(acc, "Currently building list is empty");
                    return nextTask;
                }

                NextExecute = TimeHelper.RanDelay(acc, firstComplete.Duration);
                acc.Logger.Information($"Next building will be contructed after {firstComplete.Building} - level {firstComplete.Level} complete. ({NextExecute})");
                StopFlag = true;
                return nextTask;
            }

            return nextTask;
        }

        private bool TaskTypeCondition(Account acc)
        {
            switch (_buildingTask.TaskType)
            {
                case BuildingType.General:
                    if (!UpgradeBuildingHelper.CheckGeneralTaskBuildPlace(Vill, _buildingTask))
                    {
                        acc.Logger.Warning($"Cannot find place to construct {_buildingTask.Building}. Skip this and move on next one");
                        RemoveCurrentTask();
                        _buildingTask = null;
                        return false;
                    }
                    return true;

                case BuildingType.AutoUpgradeResFields:
                    {
                        acc.Logger.Information("This is task auto upgrade res field. Choose what res fields will upgrade");
                        UpgradeBuildingHelper.AddResFields(acc, Vill, _buildingTask);
                        var task = Vill.Build.Tasks.FirstOrDefault();
                        acc.Logger.Information($"Added {task.Building} - Level {task.Level} to queue");
                        return false;
                    }
            }

            return true;
        }

        private async Task<bool> FreeCropCondition(Account acc)
        {
            acc.Logger.Information($"Checking current village ...");
            await NavigationHelper.SwitchVillage(acc, Vill);
            acc.Logger.Information($"Update free crop ...");

            if (Vill.Res.FreeCrop <= 5 && _buildingTask.Building != BuildingEnum.Cropland)
            {
                acc.Logger.Warning($"Don't have enough Free Crop for {_buildingTask.Building} - level {_buildingTask.Level}. Will upgrade Cropland instead.");

                UpgradeBuildingHelper.UpgradeBuildingForOneLvl(acc, Vill, BuildingEnum.Cropland, false);
                return false;
            }
            return true;
        }

        private async Task MoveIntoBuilding(Account acc)
        {
            await AccountHelper.DelayWait(acc);
            await NavigationHelper.EnterBuilding(acc, Vill, (int)_buildingTask.BuildingId, 0);
            var build = Vill.Build.Buildings.FirstOrDefault(x => x.Id == _buildingTask.BuildingId);
            if (build.Type == BuildingEnum.Site)
            {
                await AccountHelper.DelayWait(acc);
                acc.Logger.Information($"This is contruct task, choose correct tab for building {_buildingTask.Building}");
                await NavigationHelper.ToConstructionTab(acc, _buildingTask.Building);
            }
        }

        private bool IsContructPage(Account acc)
        {
            acc.Logger.Information($"Finding button to build ...");

            acc.Wb.UpdateHtml();
            var contractNode = acc.Wb.Html.GetElementbyId($"contract_building{(int)_buildingTask.Building}");
            if (contractNode != null)
            {
                construct = true;
            }
            else
            {
                contractNode = acc.Wb.Html.GetElementbyId("build");
                if (contractNode != null)
                {
                    construct = false;
                }
                else
                {
                    Retry(acc, "Couldn't find");
                    return false;
                }
            }
            acc.Logger.Information("Found it!");
            return true;
        }

        private async Task<bool> IsEnoughRes(Account acc)
        {
            // check enough res
            acc.Wb.UpdateHtml();
            HtmlNode contractNode = null;
            if (construct)
            {
                contractNode = acc.Wb.Html.GetElementbyId($"contract_building{(int)_buildingTask.Building}");
            }
            else
            {
                contractNode = acc.Wb.Html.GetElementbyId("contract");
            }

            var resWrapper = contractNode.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var cost = ResourceParser.GetResourceCost(resWrapper);

            acc.Logger.Information($"Need {cost}");

            if (!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
            {
                if (ResourcesHelper.IsStorageTooLow(acc, Vill, cost))
                {
                    var building = Vill.Build.CurrentlyBuilding.FirstOrDefault(x => x.Building == BuildingEnum.Warehouse || x.Building == BuildingEnum.Granary);
                    if (building == null)
                    {
                        acc.Logger.Warning($"Storage is too low. Added storage upgrade.");
                    }
                    else
                    {
                        acc.Logger.Warning($"Storage is too low. Next building will be contructed after {building.Building} - level {building.Level} complete. ({NextExecute})");
                        NextExecute = TimeHelper.RanDelay(acc, building.Duration);
                        StopFlag = true;
                    }
                    return false;
                }

                var stillNeededRes = ResourcesHelper.SubtractResources(cost.ToArray(), Vill.Res.Stored.Resources.ToArray(), true);
                acc.Logger.Information("Not enough resources to build.");
                if (Vill.Settings.UseHeroRes && acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5) // Only T4.5 has resources in hero inv
                {
                    var heroRes = HeroHelper.GetHeroResources(acc);

                    if (ResourcesHelper.IsEnoughRes(heroRes, stillNeededRes))
                    {
                        // If we have enough hero res for our task, execute the task
                        // right after hero equip finishes
                        acc.Logger.Information("Bot will use resource from hero inventory");

                        var heroEquipTask = ResourcesHelper.UseHeroResources(acc, Vill, ref stillNeededRes, heroRes, _buildingTask);
                        await heroEquipTask.Execute(acc);
                        return false;
                    }
                }

                acc.Logger.Information($"Bot will try finish the task later");
                DateTime enoughRes = TimeHelper.EnoughResToUpgrade(Vill, stillNeededRes);
                NextExecute = TimeHelper.RanDelay(acc, enoughRes);
                StopFlag = true;
                return false;
            }
            return true;
        }
    }
}