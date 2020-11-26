

using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
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
            // Sets building task to be built
            //if (this.Task == null)
            ConfigNextExecute(acc);

            this.NextExecute = DateTime.Now.AddMinutes(2);
            if (this.Task == null)
            {
                // There is no building task left. Remove the BotTask
                acc.Tasks.Remove(this);
                return TaskRes.Executed;
            }

            // Check if the task is complete
            var urlId = BuildingHelper.GetUrlForBuilding(Vill, Task);
            if (urlId == null)
            {
                //no space for this building
                Vill.Build.Tasks.Remove(this.Task);
                this.Task = null;
                return await Execute(acc);
            }

            // In which dorf is the building. So bot is less suspicious.
            if (!acc.Wb.CurrentUrl.Contains($"/dorf{((Task.BuildingId ?? default) < 19 ? 1 : 2)}.php"))
            {
                string navigateTo = $"{acc.AccInfo.ServerUrl}/";
                //Switch village!
                navigateTo += (Task.BuildingId ?? default) < 19 ?
                    "dorf1.php" :
                    "dorf2.php";

                // For localization purposes, bot sends raw http req to Travian servers. 
                // We need localized building names, and JS hides the title of the 
                //buildings on selenium browser.
                acc.Wb.Html = await HttpHelper.SendGetReq(acc, navigateTo);
                await System.Threading.Tasks.Task.Delay(AccountHelper.Delay());

                if (navigateTo.EndsWith("dorf1.php")) TaskExecutor.UpdateDorf1Info(acc);
                else TaskExecutor.UpdateDorf2Info(acc); // dorf2 ok

                Localizations.UpdateLocalization(acc);
            }

            // Check if there are already too many buildings currently constructed
            var maxBuild = 1;
            if (acc.AccInfo.PlusAccount) maxBuild++;
            if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild++;
            if (Vill.Build.CurrentlyBuilding.Count >= maxBuild)
            {
                //Execute next upgrade task after currently building
                this.NextExecute = Vill.Build.CurrentlyBuilding.First().Duration.AddSeconds(3);
                TaskExecutor.ReorderTaskList(acc);
                return TaskRes.Executed;
            }


            var url = $"{acc.AccInfo.ServerUrl}/build.php?id={urlId}";

            // Fast building for TTWars, only if we have enough resources
            //if (acc.AccInfo.ServerUrl.Contains("ttwars") && !url.Contains("category") && false) // disabling this
            //{
            //    var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == this.task.BuildingId);
            //    var lvl = building.Level;
            //    if (building.UnderConstruction) lvl++;

            //    var storedRes = ResourcesHelper.ResourcesToArray(vill.Res.Stored.Resources);
            //    var neededRes = BuildingsCost.GetBuildingCost(this.task.Building, lvl + 1);
            //    if (ResourcesHelper.EnoughRes(storedRes, neededRes))
            //    {
            //        //url += "&fastUP=1";
            //        return TaskRes.Executed;
            //    }
            //}

            // Append correct tab
            switch (this.Task.Building)
            {
                case BuildingEnum.RallyPoint:
                    url += "&tt=0";
                    break;
                case BuildingEnum.Marketplace:
                    url += "&t=0";
                    break;
            }
            await acc.Wb.Navigate(url);

            // Check if enough resources
            //BuildingsCost.GetBuildingCost()

            this.NextExecute = DateTime.Now.AddMinutes(2);

            var contractBuilding = acc.Wb.Html.GetElementbyId($"contract_building{(int)Task.Building}");
            var upgradeBuildingContract = acc.Wb.Html.GetElementbyId("build");
            TaskRes res;
            if (contractBuilding != null) //Construct a new building
                res = await Construct(acc, contractBuilding);
            else if (upgradeBuildingContract != null) // Upgrade building
                res = await Upgrade(acc, upgradeBuildingContract);
            else
                throw new Exception("No construct or upgrade contract was found!");

            ConfigNextExecute(acc);
            return res;
        }

        /// <summary>
        /// Building isn't constructed yet - construct it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="contractBuilding">HtmlNode</param>
        /// <returns>TaskResult</returns>
        private async Task<TaskRes> Construct(Account acc, HtmlNode contractBuilding)
        {
            var resWrapper = contractBuilding.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var res = ResourceParser.GetResourceCost(resWrapper);

            var nextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, res);

            if (nextExecute < DateTime.Now.AddMilliseconds(1)) // we have enough res, go construct that building boii
            {
                var button = contractBuilding.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

                // Check for prerequisites
                if (button == null)
                {
                    // Add prerequisite buildings in order to construct this building.
                    if (!AddBuildingPrerequisites(acc, Vill, Task.Building, false))
                    {
                        // Next execute after the last building finishes
                        this.NextExecute = Vill.Build.CurrentlyBuilding.LastOrDefault()?.Duration;
                        return TaskRes.Executed;
                    }
                    else
                    {
                        acc.Wb.Log($"Error trying to construct {Task.Building}! Prerequisites are met but there is no 'Build' button!");

                        return TaskRes.Retry;
                    }
                }

                await acc.Wb.Driver.FindElementById(button.Id).Click(acc);
                this.Task.ConstructNew = false;

                CheckIfTaskFinished(1);

                acc.Wb.Log($"Started construction of {this.Task.Building} in {this.Vill?.Name}");

                await PostTaskCheckDorf(acc);

                return TaskRes.Executed;
            }
            //not enough resources, wait until resources get produced/transited from main village
            this.NextExecute = nextExecute;
            return TaskRes.Executed;
        }

        /// <summary>
        /// Building is already constructed, upgrade it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="node">HtmlNode</param>
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
            if (lvl >= Task.Level)
            {
                acc.Wb.Log($"{this.Task.Building} is on level {lvl}, on/above desired {Task.Level}. Removing it from queue.");
                RemoveCurrentTask();
                RemoveCompletedTasks(this.Vill, acc);
                return TaskRes.Executed;
            }

            var container = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
            var buttons = container?.Descendants("button");
            if (buttons == null)
                throw new Exception($"We wanted to upgrade {Task.Building}, but no 'upgrade' button was found!");

            var errorMessage = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("errorMessage"));
            HtmlNode upgradeButton = buttons.FirstOrDefault(x => x.HasClass("build"));

            // Not enough resources
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.T4_4:
                    if (upgradeButton == null) return NotEnoughRes(acc);
                    break;
                case ServerVersionEnum.T4_5:
                    if (errorMessage != null) return NotEnoughRes(acc);
                    break;
            }

            // TODO: check if there is no upgrade button due to already building another building

            var buildDuration = InfrastructureParser.GetBuildDuration(container, acc.AccInfo.ServerVersion);

            if (IsTaskCompleted(Vill, acc, this.Task))
            {
                acc.Wb.Log($"Building {this.Task.Building} in village {this.Vill.Name} is already on desired level. Will be removed from the queue.");
                RemoveCurrentTask();
                return TaskRes.Executed;
            }
            //TODO move this
            CheckSettlers(acc, Vill, lvl, DateTime.Now.Add(buildDuration));

            // +25% speed upgrade
            //{
            //    if (await DriverHelper.ExecuteScript(acc, "document.getElementsByClassName('videoFeatureButton green')[0].click();", false))
            //    {
            //        // wait
            //        //acc.Wb.Driver.FindElementsByClassName("videoFeatureButton").First().Click();
            //    }
            //    else 
            //}
            await acc.Wb.Driver.FindElementById(upgradeButton.Id).Click(acc);

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
            if (this.Task.Level <= lvl) RemoveCurrentTask();
        }

        private void RemoveCurrentTask() => this.Vill.Build.Tasks.Remove(this.Task);

        private TaskRes NotEnoughRes(Account acc)
        {
            var contract = acc.Wb.Html.GetElementbyId("contract");
            var resWrapper = contract.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var res = ResourceParser.GetResourceCost(resWrapper);
            var nextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, res, this.Task);
            acc.Wb.Log($"Not enough resources for the building! Next execute in {(nextExecute - DateTime.Now).TotalSeconds} sec");
            this.NextExecute = nextExecute;
            return TaskRes.Retry;
        }

        private async Task PostTaskCheckDorf(Account acc) => await TaskExecutor.PageLoaded(acc);


        //TODO: Have this as postCheck? just so it doesn't get constantly checked
        private void CheckSettlers(Account acc, Village vill, int currentLevel, DateTime finishBuilding)
        {
            if (this.Task.Building == BuildingEnum.Residence &&
                currentLevel >= 9 &&
                acc.NewVillages.AutoSettleNewVillages &&
                vill.Troops.Settlers == 0)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, vill,
                    new TrainSettlers()
                    {
                        ExecuteAt = finishBuilding.AddSeconds(5),
                        Vill = vill
                    });
            }
        }

        /// <summary>
        /// Configures the UpgradeBuilding BotTask for the next execution. It should select the building (if autoRes),
        /// configure correct time and get correct id if it doesn't exist yet.
        /// </summary>
        /// <param name="acc">Account</param>
        public void ConfigNextExecute(Account acc)
        {
            RemoveFinishedCB(Vill);

            if (Vill.Build.AutoBuildResourceBonusBuildings) CheckResourceBonus(Vill);

            // Checks if we have enough FreeCrop (above 0)
            CheckFreeCrop();

            // Worst case: leave nextExecute as is (after the current building finishes)
            // Best case: now
            (var nextTask, var time) = FindBuildingTask(acc, Vill);
            
            if (nextTask == null) return;

            this.Task = nextTask;

            Random ran = new Random();

            var upperLimitSec = 60;
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_4) upperLimitSec = 3;
            this.NextExecute = time.AddSeconds(ran.Next(1, upperLimitSec));
            //Console.WriteLine($"-------Next build execute: {this.task?.Building}, in {((this.NextExecute ?? DateTime.Now) - DateTime.Now).TotalSeconds}s");
        }

        /// <summary>
        /// Checks if we have enough free crop in the village (otherwise we can't upgrade any building)
        /// </summary>
        private void CheckFreeCrop()
        {
            // 5 is maximum a building can take up free crop (stable lvl 1)
            if (this.Vill.Res.FreeCrop <= 5 && Vill.Build.Tasks.FirstOrDefault()?.Building != BuildingEnum.Cropland)
            {
                var croplandsInVill = Vill.Build.Buildings.Where(x => x.Type == BuildingEnum.Cropland).ToList();
                var cropland = FindLowestLevelBuilding(croplandsInVill);
                var CB = cropland.UnderConstruction ? 1 : 0;

                var cropTask = new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = BuildingEnum.Cropland,
                    Level = cropland.Level + 1 + CB,
                    BuildingId = cropland.Id
                };
                
                Vill.Build.Tasks.Insert(0, cropTask);
            }
        }

        /// <summary>
        /// After fast upgrading, check if browser is inside the building view. If so, building is already on max and should be removed from the buildingTask list.
        /// This will get called before the ConfigureNextExecute.
        /// </summary>
        /// <param name="htmlDoc">Html</param>
        /// <param name="acc">Account</param>
        public void PostCheckFastUpgrade(HtmlDocument htmlDoc, Account acc)
        {
            var upgradeBuildingContract = htmlDoc.GetElementbyId("build");
            if (upgradeBuildingContract != null) // Building is already on max level. Remove it from queue.
            {
                Vill.Build.Tasks.Remove(this.Task);
            }
        }


        private void CheckResourceBonus(Village vill)
        {
            //For auto building resource bonus buildings
            if (vill.Build.AutoBuildResourceBonusBuildings &&
                vill.Build.Buildings.Any(x => x.Type == BuildingEnum.MainBuilding && x.Level >= 5)) // If enabled and MainBuilding is above lvl 5
            {
                var bonusBuilding = CheckBonusBuildings(vill);
                if (bonusBuilding != BuildingEnum.Site)
                {
                    var bonusTask = new BuildingTask()
                    {
                        TaskType = BuildingType.General,
                        Building = bonusBuilding,
                        Level = 5,
                    };

                    if (BuildingHelper.FindBuildingId(vill, bonusTask))
                    {
                        vill.Build.Tasks.Insert(0, bonusTask);
                    }
                }
            }
        }

        private (BuildingTask, DateTime) FindBuildingTask(Account acc, Village vill)
        {
            if (vill.Build.Tasks.Count == 0) return (null, DateTime.Now);

            var now = DateTime.Now.AddHours(-2); // Since we are already in the village
            var later = DateTime.Now.AddMilliseconds(500);
            var totalBuild = vill.Build.CurrentlyBuilding.Count;
            if (totalBuild > 0) later = vill.Build.CurrentlyBuilding.First().Duration;

            var maxBuild = 1;
            if (acc.AccInfo.PlusAccount) maxBuild++;
            if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild++;

            BuildingTask task = null;

            // If (roman OR ttwars+plus acc) -> build 1 res + 1 infra at the same time
            if (1 <= totalBuild &&
               (acc.AccInfo.Tribe == TribeEnum.Romans ||
               (acc.AccInfo.PlusAccount && acc.AccInfo.ServerUrl.ToLower().Contains("ttwars"))
                ))
            {
                //find the CurrentlyBuilding that executes sooner
                var cb = vill.Build.CurrentlyBuilding.OrderBy(x => x.Duration).First();
                later = cb.Duration;

                var isResField = IsResourceField(cb.Building);

                task = isResField ? GetFirstInfrastructureTask(vill) : GetFirstResTask(vill);
                
                if (task != null) return (task, now);
                maxBuild--;
            } 
            
            task = vill.Build.Tasks.First();

            //If this task is already complete, remove it and repeat the finding process
            if (IsTaskCompleted(vill, acc, task))
            {
                vill.Build.Tasks.Remove(task); //task has been completed
                return FindBuildingTask(acc, vill);
            }

            //if buildingId is not yet defined, find one.
            if (task.BuildingId == null && task.TaskType == BuildingType.General)
            {
                var found = FindBuildingId(vill, task);
                //no space found for this building, remove the buildTask
                if (!found)
                {
                    vill.Build.Tasks.Remove(task);
                    return FindBuildingTask(acc, vill);
                }
            }

            if (totalBuild < maxBuild) return (task, now);
            else return (task, later);
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
            if (BonusHelper(vill, BuildingEnum.Cropland, BuildingEnum.Bakery, 10) && vill.Build.Buildings.FirstOrDefault(x => x.Type == BuildingEnum.GrainMill && x.Level >= 5) != null)
                return BuildingEnum.Bakery;

            return BuildingEnum.Site;
        }

        private bool BonusHelper(Village vill, BuildingEnum field, BuildingEnum bonus, int fieldLvl) // vill does not have bonus building on 5, create or upgrade it
        {
            //res field is high enoguh, bonus building is not on 5, there is still space left to build, there isn't already a bonus building buildtask
            return (!vill.Build.Buildings.Any(x => x.Type == bonus && x.Level >= 5) &&
                vill.Build.Buildings.Any(x => x.Type == field && x.Level >= fieldLvl) &&
                vill.Build.Buildings.Any(x => x.Type == Classificator.BuildingEnum.Site) &&
                !vill.Build.Tasks.Any(x => x.Building == bonus));
        }

        private BuildingTask GetFirstResTask(Village vill)
        {
            return vill.Build.Tasks.FirstOrDefault(x =>
            x.TaskType == BuildingType.AutoUpgradeResFields || IsResourceField(x.Building)
            );
        }
        private BuildingTask GetFirstInfrastructureTask(Village vill)
        {
            return vill.Build.Tasks.FirstOrDefault(x =>
            x.TaskType != BuildingType.AutoUpgradeResFields && !IsResourceField(x.Building)
            );
        }
    }
}