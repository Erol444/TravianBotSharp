using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.TravianData;
using static TravBotSharp.Files.Helpers.BuildingHelper;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpgradeBuilding : BotTask
    {
        public BuildingTask Task { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            ConfigNextExecute(acc, false);

            if (this.Task == null)
            {
                // There is no building task left. Remove the BotTask
                acc.Tasks.Remove(this);
                return TaskRes.Executed;
            }

            // Check if the task is complete
            var (urlId, constructNew) = GetUrlForBuilding(acc, Vill, Task);
            if (urlId == null)
            {
                //no space for this building
                Vill.Build.Tasks.Remove(this.Task);
                this.Task = null;
                return await Execute(acc);
            }

            // Check if there are already too many buildings currently constructed
            var maxBuild = 1;
            if (acc.AccInfo.PlusAccount) maxBuild++;
            if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild++;
            if (maxBuild <= Vill.Build.CurrentlyBuilding.Count)
            {
                //Execute next upgrade task after currently building
                this.NextExecute = Vill.Build.CurrentlyBuilding.First().Duration.AddSeconds(3);
                TaskExecutor.ReorderTaskList(acc);
                return TaskRes.Executed;
            }

            var url = $"{acc.AccInfo.ServerUrl}/build.php?id={urlId}";

            // Fast building for TTWars
            if (acc.AccInfo.ServerUrl.Contains("ttwars") &&
                !constructNew &&
                await TTWarsTryFastUpgrade(acc, url))
            {
                return TaskRes.Executed;
            }

            // Navigate to the dorf in which the building is, so bot is less suspicious
            string dorfUrl = $"/dorf{((Task.BuildingId ?? default) < 19 ? 1 : 2)}.php"; // "dorf1" / "dorf2"
            if (!acc.Wb.CurrentUrl.Contains(dorfUrl))
            {
                await acc.Wb.Navigate(acc.AccInfo.ServerUrl + dorfUrl);
            }

            // Append correct tab
            if (!constructNew)
            {
                switch (this.Task.Building)
                {
                    case BuildingEnum.RallyPoint:
                        url += "&tt=0";
                        break;

                    case BuildingEnum.Marketplace:
                        url += "&t=0";
                        break;

                    case BuildingEnum.Residence:
                    case BuildingEnum.Palace:
                    case BuildingEnum.CommandCenter:
                    case BuildingEnum.Treasury:
                        url += "&s=0";
                        break;
                }
            }

            await acc.Wb.Navigate(url);

            var constructContract = acc.Wb.Html.GetElementbyId($"contract_building{(int)Task.Building}");
            var upgradeContract = acc.Wb.Html.GetElementbyId("build");

            TaskRes response;
            this.NextExecute = null;

            if (constructContract != null)
            {
                if (!IsEnoughRes(acc, constructContract)) return TaskRes.Executed;
                response = await Construct(acc, constructContract);
            }
            else if (upgradeContract != null)
            {
                if (!IsEnoughRes(acc, upgradeContract)) return TaskRes.Executed;
                response = await Upgrade(acc, upgradeContract);
            }
            else throw new Exception("No contract was found!");

            if (this.NextExecute == null) ConfigNextExecute(acc);
            return response;
        }

        /// <summary>
        /// Building isn't constructed yet - construct it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskResult</returnss>
        private async Task<TaskRes> Construct(Account acc, HtmlNode node)
        {
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button == null)
            {
                // Add prerequisite buildings in order to construct this building.
                AddBuildingPrerequisites(acc, Vill, Task.Building, false);

                // Next execute after the last building finishes
                this.NextExecute = Vill.Build.CurrentlyBuilding.LastOrDefault()?.Duration;

                acc.Wb.Log($"Wanted to construct {this.Task.Building} but prerequired buildings are missing");
                return TaskRes.Executed;
            }

            await DriverHelper.ClickById(acc, button.Id);

            this.Task.ConstructNew = false;

            CheckIfTaskFinished(1);

            acc.Wb.Log($"Started construction of {this.Task.Building} in {this.Vill?.Name}");

            await PostTaskCheckDorf(acc);

            return TaskRes.Executed;
        }

        /// <summary>
        /// Building is already constructed, upgrade it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskResult</returns>
        private async Task<TaskRes> Upgrade(Account acc, HtmlNode node)
        {
            (var buildingEnum, var lvl) = InfrastructureParser.UpgradeBuildingGetInfo(node);

            if (buildingEnum == BuildingEnum.Site || lvl == -1)
            {
                acc.Wb.Log($"Can't upgrade building {this.Task.Building} in village {this.Vill.Name}. Will be removed from the queue.");
                RemoveCurrentTask();
                return TaskRes.Executed;
            }

            // If there is already a different building in this spot, find a new id to construct it.
            if (buildingEnum != Task.Building)
            {
                acc.Wb.Log($"We wanted to upgrade {Task.Building}, but there's already {buildingEnum} on this id ({Task.BuildingId}).");
                if (!BuildingHelper.FindBuildingId(Vill, this.Task))
                {
                    acc.Wb.Log($"Found another Id to build {Task.Building}, new id: {Task.BuildingId}");
                    return TaskRes.Retry;
                }
                acc.Wb.Log($"Failed to find another Id to build {Task.Building}! No space in village. Building task will be removed");
                RemoveCurrentTask();
                return TaskRes.Executed;
            }

            // Basic task already on/above desired level, don't upgrade further
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Id == this.Task.BuildingId);
            lvl = building.Level;
            if (building.UnderConstruction) lvl++;
            if (Task.Level <= lvl)
            {
                acc.Wb.Log($"{this.Task.Building} is on level {lvl}, on/above desired {Task.Level}. Removing it from queue.");
                RemoveCurrentTask();
                RemoveCompletedTasks(this.Vill, acc);
                return TaskRes.Executed;
            }

            var container = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            var buttons = container?.Descendants("button");
            if (buttons == null)
            {
                acc.Wb.Log($"We wanted to upgrade {Task.Building}, but no 'upgrade' button was found! Url={acc.Wb.CurrentUrl}");
                return TaskRes.Retry;
            }

            var errorMessage = acc.Wb.Html.GetElementbyId("build")
                .Descendants("div")
                .FirstOrDefault(x => x.HasClass("upgradeBuilding"))?
                .Descendants("div")?
                .FirstOrDefault(x => x.HasClass("errorMessage"));
            HtmlNode upgradeButton = buttons.FirstOrDefault(x => x.HasClass("build"));

            if (upgradeButton == null)
            {
                acc.Wb.Log($"We wanted to upgrade {Task.Building}, but no 'upgrade' button was found!");
                return TaskRes.Retry;
            }

            // Not enough resources?
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5 && errorMessage != null)
            {
                acc.Wb.Log($"We wanted to upgrade {Task.Building}, but there was an error message:\n{errorMessage.InnerText}");
                return TaskRes.Retry;
            }

            var buildDuration = InfrastructureParser.GetBuildDuration(container, acc.AccInfo.ServerVersion);

            if (IsTaskCompleted(Vill, acc, this.Task))
            {
                acc.Wb.Log($"Building {this.Task.Building} in village {this.Vill.Name} is already on desired level. Will be removed from the queue.");
                RemoveCurrentTask();
                return TaskRes.Executed;
            }

            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_4 ||
               buildDuration.TotalMinutes <= acc.Settings.WatchAdAbove ||
               !await TryFastUpgrade(acc)) // +25% speed upgrade
            {
                await DriverHelper.ClickById(acc, upgradeButton.Id); // Normal upgrade
            }

            lvl++;
            CheckIfTaskFinished(lvl);

            acc.Wb.Log($"Started upgrading {this.Task.Building} to level {lvl} in {this.Vill?.Name}");
            await PostTaskCheckDorf(acc);

            return TaskRes.Executed;
        }

        /// <summary>
        /// This method is called after successful upgrade/construction
        /// Check if the selected task was just finished
        /// </summary>
        /// <param name="lvl">Level of the building</param>
        private void CheckIfTaskFinished(int lvl)
        {
            if (this.Task.Level <= lvl && this.Task.TaskType == BuildingType.General) RemoveCurrentTask();
        }

        private void RemoveCurrentTask() => this.Vill.Build.Tasks.Remove(this.Task);

        private async Task PostTaskCheckDorf(Account acc)
        {
            await System.Threading.Tasks.Task.Delay(AccountHelper.Delay());
            await TaskExecutor.PageLoaded(acc);

            // Check if residence is getting upgraded to level 10 => train settlers
            var cbResidence = Vill.Build
                .CurrentlyBuilding
                .FirstOrDefault(x => x.Building == BuildingEnum.Residence && x.Level == 10);

            if (cbResidence != null &&
                acc.NewVillages.AutoSettleNewVillages &&
                Vill.Troops.Settlers == 0)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, Vill,
                    new TrainSettlers()
                    {
                        ExecuteAt = cbResidence.Duration.AddSeconds(5),
                        Vill = Vill,
                        // For high speed servers, you want to train settlers asap
                        Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
                    });
            }
        }

        /// <summary>
        /// Tries to watch an Ad for +25% faster upgrade
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>Whether bot watched the ad</returns>
        private async Task<bool> TryFastUpgrade(Account acc)
        {
            if (!await DriverHelper.ClickByClassName(acc, "videoFeatureButton green", false)) return false;

            // Has to be a legit "click"
            acc.Wb.Driver.FindElementById("videoFeature").Click();

            // use script to skip ads instead of waiting, found out by Merlin#7649
            // tested with travian company's ads, not yet with 3rd company, pls send me (VINAGHOST) screenshot Debug tab

            // delay 5s for ping issue or whatever can happen =))
            await System.Threading.Tasks.Task.Delay(5000);

            //they use ifarme to emebed ads video to their game
            var iframe = acc.Wb.Driver.FindElementById("videoArea");
            acc.Wb.Log($"Ads Url: {iframe.GetAttribute("src")}");
            acc.Wb.Driver.SwitchTo().Frame(iframe);

            // trick to skip
            await DriverHelper.ExecuteScript(acc, "var video = document.getElementsByTagName('video')[0];video.currentTime = video.duration;", true, false);

            //back to first page
            acc.Wb.Driver.SwitchTo().DefaultContent();

            // wait for finish watching ads
            var timeout = DateTime.Now.AddSeconds(100);
            do
            {
                await System.Threading.Tasks.Task.Delay(1000);
                if (timeout < DateTime.Now) return false;
            }
            while (acc.Wb.Driver.Url.Contains("build.php"));

            // Don't show again
            acc.Wb.UpdateHtml();
            if (acc.Wb.Html.GetElementbyId("dontShowThisAgain") != null)
            {
                await DriverHelper.ClickById(acc, "dontShowThisAgain");
                await DriverHelper.ClickByClassName(acc, "dialogButtonOk ok");
            }
            return true;
        }

        public async Task<bool> TTWarsTryFastUpgrade(Account acc, string url)
        {
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Id == this.Task.BuildingId);
            var lvl = building.Level;
            if (building.UnderConstruction) lvl++;

            var neededRes = BuildingsData.GetBuildingCost(building.Type, lvl + 1);

            if (ResourcesHelper.IsEnoughRes(Vill, neededRes) &&
                lvl != 0 &&
                lvl < Task.Level)
            {
                await acc.Wb.Navigate(url + "&fastUP=1");

                CheckIfTaskFinished(++lvl);

                acc.Wb.Log($"Started (fast) upgrading {building.Type} to level {lvl} in {this.Vill?.Name}");

                await PostTaskCheckDorf(acc);

                ConfigNextExecute(acc);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Configures the UpgradeBuilding BotTask for the next execution. It should select the building (if autoRes),
        /// configure correct time and get correct id if it doesn't exist yet.
        /// </summary>
        /// <param name="acc">Account</param>
        public void ConfigNextExecute(Account acc, bool restart = true)
        {
            RemoveFinishedCB(Vill);

            if (Vill.Build.AutoBuildResourceBonusBuildings) CheckResourceBonus(acc, Vill, restart);

            // Checks if we have enough FreeCrop (above 0)
            CheckFreeCrop(acc);

            // Worst case: leave nextExecute as is (after the current building finishes)
            // Best case: now
            (var nextTask, var time) = UpgradeBuildingHelper.NextBuildingTask(acc, Vill);

            if (nextTask == null) return;

            this.Task = nextTask;
            this.NextExecute = TimeHelper.RanDelay(acc, time, 20);
        }

        /// <summary>
        /// Checks if we have enough resources to build the building. If we don't have enough resources,
        /// method sets NextExecute DateTime.
        /// </summary>
        /// <param name="node">Node of the contract</param>
        /// <returns>Whether we have enough resources</returns>
        private bool IsEnoughRes(Account acc, HtmlNode node)
        {
            var resWrapper = node.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var cost = ResourceParser.GetResourceCost(resWrapper).ToArray();

            // We have enough resources, go on and build it
            if (ResourcesHelper.IsEnoughRes(Vill.Res.Stored.Resources.ToArray(), cost)) return true;

            ResourcesHelper.NotEnoughRes(acc, Vill, cost, this, this.Task);

            return false;
        }

        /// <summary>
        /// Checks if we have enough free crop in the village (otherwise we can't upgrade any building)
        /// </summary>
        private void CheckFreeCrop(Account acc)
        {
            // 5 is maximum a building can take up free crop (stable lvl 1)
            if (this.Vill.Res.FreeCrop <= 5 && Vill.Build.Tasks.FirstOrDefault()?.Building != BuildingEnum.Cropland)
            {
                UpgradeBuildingForOneLvl(acc, this.Vill, BuildingEnum.Cropland, false);
            }
        }

        /// <summary>
        /// For auto-building resource bonus building
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        private void CheckResourceBonus(Account acc, Village vill, bool restart = true)
        {
            // If enabled and MainBuilding is above level 5
            if (vill.Build.Buildings.Any(x => x.Type == BuildingEnum.MainBuilding && 5 <= x.Level))
            {
                var bonusBuilding = CheckBonusBuildings(vill);
                if (bonusBuilding == BuildingEnum.Site) return;

                var bonusTask = new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = bonusBuilding,
                    Level = 5,
                };

                AddBuildingTask(acc, vill, bonusTask, false, restart);
            }
        }

        private BuildingEnum CheckBonusBuildings(Village vill)
        {
            if (BonusHelper(vill, BuildingEnum.Woodcutter, BuildingEnum.Sawmill, 10))
                return BuildingEnum.Sawmill;
            if (BonusHelper(vill, BuildingEnum.ClayPit, BuildingEnum.Brickyard, 10))
                return BuildingEnum.Brickyard;
            if (BonusHelper(vill, BuildingEnum.IronMine, BuildingEnum.IronFoundry, 10))
                return BuildingEnum.IronFoundry;
            if (BonusHelper(vill, BuildingEnum.Cropland, BuildingEnum.GrainMill, 5))
                return BuildingEnum.GrainMill;
            if (BonusHelper(vill, BuildingEnum.Cropland, BuildingEnum.Bakery, 10) &&
                vill.Build.Buildings.Any(x => x.Type == BuildingEnum.GrainMill && x.Level >= 5))
                return BuildingEnum.Bakery;

            return BuildingEnum.Site;
        }

        private bool BonusHelper(Village vill, BuildingEnum field, BuildingEnum bonus, int fieldLvl) // vill does not have bonus building on 5, create or upgrade it
        {
            //res field is high enoguh, bonus building is not on 5, there is still space left to build, there isn't already a bonus building buildtask
            return (!vill.Build.Buildings.Any(x => x.Type == bonus && x.Level >= 5) &&
                vill.Build.Buildings.Any(x => x.Type == field && x.Level >= fieldLvl) &&
                vill.Build.Buildings.Any(x => x.Type == BuildingEnum.Site) &&
                !vill.Build.Tasks.Any(x => x.Building == bonus));
        }
    }
}