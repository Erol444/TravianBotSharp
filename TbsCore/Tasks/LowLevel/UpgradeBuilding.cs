

using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;
using static TravBotSharp.Files.Helpers.BuildingHelper;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpgradeBuilding : BotTask
    {
        private BuildingTask task;

        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            if (this.task == null)
            {
                ConfigNextExecute(acc.Wb.Html, acc);
                this.NextExecute = DateTime.Now.AddMinutes(2);
                if (this.task == null)
                {
                    // There is no building task left. Remove the BotTask
                    acc.Tasks.Remove(this);
                    return TaskRes.Executed;
                }
            }

            // Check if the task is complete
            var urlId = BuildingHelper.GetUrlForBuilding(vill, task);
            if (urlId == null)
            {
                //no space for this building
                vill.Build.Tasks.Remove(this.task);
                this.task = null;
                return await Execute(htmlDoc, wb, acc);
            }

            // In which dorf is the building. Maybe not needed.
            if (!acc.Wb.CurrentUrl.Contains($"/dorf{((task.BuildingId ?? default) < 19 ? 1 : 2)}.php"))
            {
                //Switch village!
                BotTask updateDorfTask = (task.BuildingId ?? default) < 19 ?
                    (BotTask)new UpdateDorf1() :
                    (BotTask)new UpdateDorf2();

                updateDorfTask.vill = this.vill;
                updateDorfTask.ExecuteAt = DateTime.MinValue.AddHours(2);

                TaskExecutor.AddTask(acc, updateDorfTask);
                this.NextExecute = updateDorfTask.ExecuteAt.AddMinutes(1);
                return TaskRes.Executed;
            }

            // Check if there are already too many buildings currently constructed
            BuildingHelper.RemoveFinishedCB(vill);
            var maxBuild = 1;
            if (acc.AccInfo.PlusAccount) maxBuild++;
            if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild++;
            if (vill.Build.CurrentlyBuilding.Count >= maxBuild)
            {
                //Execute next upgrade task after currently building
                this.NextExecute = vill.Build.CurrentlyBuilding.First().Duration.AddSeconds(3);
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

            //append correct tab
            switch (this.task.Building)
            {
                case BuildingEnum.RallyPoint:
                    url += "&tt=0";
                    break;
                case BuildingEnum.Marketplace:
                    url += "&t=0";
                    break;
            }
            await acc.Wb.Navigate(url);

            this.PostTaskCheck.Add(ConfigNextExecute);
            this.NextExecute = DateTime.Now.AddMinutes(2);

            var contractBuilding = htmlDoc.GetElementbyId($"contract_building{(int)task.Building}");
            var upgradeBuildingContract = htmlDoc.GetElementbyId("build");
            if (contractBuilding != null) //Construct a new building
            {
                var resWrapper = contractBuilding.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
                var res = ResourceParser.GetResourceCost(resWrapper);

                var nextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, vill, res);

                if (nextExecute < DateTime.Now.AddMilliseconds(1)) // we have enough res, go construct that building boii
                {
                    var button = contractBuilding.Descendants("button").FirstOrDefault(x => x.HasClass("new"));
                    //TODO: if button null: check for prerequisites. Maybe a prerequisit is currently building...
                    if (button == null)
                    {
                        this.NextExecute = vill.Build.CurrentlyBuilding.LastOrDefault()?.Duration;
                        this.PostTaskCheck.Remove(ConfigNextExecute);
                        return TaskRes.Executed;
                    }
                    //check if button is null!
                    wb.ExecuteScript($"document.getElementById('{button.Id}').click()");
                    this.task.ConstructNew = false;

                    return TaskRes.Executed;
                }
                //not enough resources, wait until resources get produced/transited from main village
                this.NextExecute = nextExecute;
                this.PostTaskCheck.Remove(ConfigNextExecute);
                return TaskRes.Executed;
            }
            else if (upgradeBuildingContract != null) // Upgrade building
            {
                (var buildingEnum, var lvl) = InfrastructureParser.UpgradeBuildingGetInfo(upgradeBuildingContract);

                if (buildingEnum == BuildingEnum.Site || lvl == -1)
                {
                    this.ErrorMessage = $"Can't upgrade building {this.task.Building} in village {this.vill.Name}. Will be removed from the queue.";
                    vill.Build.Tasks.Remove(this.task);
                    return TaskRes.Executed;
                }

                // If there is already a different building in this spot, find a new id to construct it.
                if (buildingEnum != task.Building)
                {
                    if (!BuildingHelper.FindBuildingId(vill, task))
                    {
                        vill.Build.Tasks.Remove(this.task);
                    }
                    return TaskRes.Retry;
                }

                // Basic task already on/above desired level, don't upgrade further
                var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == this.task.BuildingId);
                if (building.UnderConstruction) lvl++;
                if (lvl >= task.Level)
                {
                    this.ErrorMessage = $"{this.task.Building} is on level {lvl}, above desired {task.Level}. Removing it from queue.";
                    vill.Build.Tasks.Remove(this.task);
                    RemoveCompletedTasks(this.vill, acc);
                    return TaskRes.Executed;
                }

                var container = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));
                var buttons = container?.Descendants("button");
                if (buttons == null)
                {
                    this.ErrorMessage = "No 'upgrade' button found!";
                    return TaskRes.Executed;
                }
                HtmlNode upgradeButton = buttons.FirstOrDefault(x => x.HasClass("build"));
                //We have enough resources, click on build
                if (upgradeButton != null)
                {
                    var buildDuration = InfrastructureParser.GetBuildDuration(container, acc.AccInfo.ServerVersion);

                    if (IsTaskCompleted(vill, acc, this.task))
                    {
                        this.ErrorMessage = $"Building {this.task.Building} in village {this.vill.Name} is already done. Will be removed from the queue.";
                        vill.Build.Tasks.Remove(this.task);
                        return TaskRes.Executed;
                    }
                    //TODO move this
                    CheckSettlers(acc, vill, lvl, DateTime.Now.Add(buildDuration));

                    Console.WriteLine($"Village {vill.Name} will upgrade {task.Building}");
                    wb.ExecuteScript($"document.getElementById('{upgradeButton.Id}').click()");
                    return TaskRes.Executed;
                }
                else
                {
                    HtmlNode error = container.Descendants("span").FirstOrDefault(x => x.HasClass("none"));
                    //Not enough resources
                    var contract = htmlDoc.GetElementbyId("contract");
                    var resWrapper = contract.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
                    var res = ResourceParser.GetResourceCost(resWrapper);
                    this.PostTaskCheck.Remove(ConfigNextExecute);
                    this.NextExecute = ResourcesHelper.EnoughResourcesOrTransit(acc, vill, res);
                    return TaskRes.Executed;
                }
            }
            else
            {
                throw new Exception("No construct or upgrade contract was found!");
            }
        }

        //TODO: Have this as postCheck? just so it doesn't get constantly checked
        private void CheckSettlers(Account acc, Village vill, int currentLevel, DateTime finishBuilding)
        {
            if (this.task.Building == BuildingEnum.Residence &&
                currentLevel >= 9 &&
                acc.NewVillages.AutoSettleNewVillages &&
                vill.Troops.Settlers == 0)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, vill,
                    new TrainSettlers()
                    {
                        ExecuteAt = finishBuilding.AddSeconds(5),
                        vill = vill,
                        Priority = TaskPriority.Medium
                    });
            }
        }

        /// <summary>
        /// Configures the UpgradeBuilding BotTask for the next execution. It should select the building (if autoRes),
        /// configure correct time and get correct id if it doesn't exist yet.
        /// </summary>
        /// <param name="htmlDoc">Html document of the page</param>
        /// <param name="acc">Account</param>
        public void ConfigNextExecute(HtmlDocument htmlDoc, Account acc)
        {
            // Worst case: leave nextExecute as is (after the current building finishes)
            // Best case: now
            CheckResourceBonus(vill);
            (var nextTask, var time) = FindBuildingTask(acc, vill);
            if (nextTask == null)
            {
                return;
            }
            this.task = nextTask;
            this.NextExecute = time.AddSeconds(1);
            //Console.WriteLine($"-------Next build execute: {this.task?.Building}, in {((this.NextExecute ?? DateTime.Now) - DateTime.Now).TotalSeconds}s");
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
                vill.Build.Tasks.Remove(this.task);
            }
        }


        private void CheckResourceBonus(Village vill)
        {
            //For auto building resource bonus buildings
            if (vill.Build.AutoBuildResourceBonusBuildings &&
                vill.Build.Buildings.Any(x => x.Type == BuildingEnum.MainBuilding && x.Level >= 5)) //if enabled and MainBuilding is above lvl 5
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

            BuildingTask task = null;

            //if (roman OR ttwars+plus acc) -> build 1 res + 1 infra at the same time :3
            if ((acc.AccInfo.Tribe == TribeEnum.Romans || (acc.AccInfo.PlusAccount && acc.AccInfo.ServerUrl.Contains("ttwars"))) &&
                totalBuild > 0)
            {
                //find the CurrentlyBuilding that executes sooner
                var cb = vill.Build.CurrentlyBuilding.OrderBy(x => x.Duration).First();
                later = cb.Duration;

                var isResField = IsResourceField(cb.Building);

                // If we are currently building only 1 building, get the opposite of what we are currently building, otherwise wait for what finishes sooner.
                bool upgradeDorf2 = totalBuild == 1 ? isResField : !isResField;

                task = upgradeDorf2 ? GetFirstInfrastructureTask(vill) : GetFirstResTask(vill);

                if (task != null)
                {
                    return totalBuild == 1 ? (task, now) : (task, later);
                }
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

        private BuildingDorf GetBuildingDorf(BuildingTask task)
        {
            bool isRes = task.TaskType == BuildingType.AutoUpgradeResFields ||
                            IsResourceField(task.Building);

            return isRes ? BuildingDorf.Dorf1 : BuildingDorf.Dorf2;
        }
    }
}