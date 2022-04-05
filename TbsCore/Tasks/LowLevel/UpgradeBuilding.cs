using HtmlAgilityPack;
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

        public override async Task<TaskRes> Execute(Account acc)
        {
            do
            {
                var nextTask = UpgradeBuildingHelper.NextBuildingTask(acc, Vill);

                if (nextTask == null)
                {
                    if (Vill.Build.Tasks.Count == 0)
                    {
                        acc.Logger.Information("Building queue empty.", this);
                        return TaskRes.Executed;
                    }

                    await SwitchVillage(acc);

                    var firstComplete = Vill.Build.CurrentlyBuilding.FirstOrDefault();
                    NextExecute = TimeHelper.RanDelay(acc, firstComplete.Duration);
                    acc.Logger.Information($"Next building will be contructed after {firstComplete.Building} - level {firstComplete.Level} complete. ({NextExecute})", this);
                    return TaskRes.Executed;
                }

                _buildingTask = nextTask;

                // check place to construct or upgrade
                switch (_buildingTask.TaskType)
                {
                    case BuildingType.General:
                        if (!UpgradeBuildingHelper.CheckGeneralTaskBuildPlace(Vill, _buildingTask))
                        {
                            acc.Logger.Warning($"Don't have slot to construct {_buildingTask.Building}. Will skip this and move on next one", this);
                            RemoveCurrentTask();
                            _buildingTask = null;
                            continue;
                        }
                        break;

                    case BuildingType.AutoUpgradeResFields:
                        {
                            UpgradeBuildingHelper.AddResFields(acc, Vill, _buildingTask);
                            continue;
                        }
                }

                await SwitchVillage(acc);

                if (!EnoughFreeCrop(acc))
                {
                    acc.Logger.Warning($"Don't have enough Free Crop for {_buildingTask.Building} - level {_buildingTask.Level}. Will upgrade Cropland instead.", this);
                    continue;
                }
                // Fast building for TTWars
                if (acc.AccInfo.ServerVersion == ServerVersionEnum.TTwars &&
                    !_buildingTask.ConstructNew)
                {
                    var fastUpgrade = await TTWarsTryFastUpgrade(acc, $"{acc.AccInfo.ServerUrl}/build.php?id={_buildingTask.BuildingId}");
                    if (fastUpgrade) continue;
                }

                await NavigationHelper.EnterBuilding(acc, Vill, (int)_buildingTask.BuildingId);
                if (_buildingTask.ConstructNew)
                {
                    await NavigationHelper.ToConstructionTab(acc, _buildingTask.Building);
                }

                // find button to contruct/upgrade
                bool construct;
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
                        acc.Logger.Warning($"Cannot find button to build {_buildingTask.Building} - Level {_buildingTask.Level}!", this);
                        continue;
                    }
                }

                // check enough res
                var cost = ResourceParser.ParseResourcesNeed(contractNode);
                if (!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
                {
                    if (ResourcesHelper.IsStorageTooLow(acc, Vill, cost))
                    {
                        acc.Logger.Warning($"Storage is too low to construct {_buildingTask.Building} - Level {_buildingTask.Level}! Needed {cost}. Bot will build storage first", this);
                        continue;
                    }
                    var stillNeededRes = ResourcesHelper.SubtractResources(cost.ToArray(), Vill.Res.Stored.Resources.ToArray(), true);

                    if (!Vill.Settings.UseHeroRes ||
                    acc.AccInfo.ServerVersion != ServerVersionEnum.T4_5) // Only T4.5 has resources in hero inv
                    {
                        acc.Logger.Warning($"Not enough resources to construct {_buildingTask.Building} - Level {_buildingTask.Level}! Needed {cost}. Bot will try finish the task later", this);
                        DateTime enoughRes = TimeHelper.EnoughResToUpgrade(Vill, stillNeededRes);
                        NextExecute = TimeHelper.RanDelay(acc, enoughRes);
                        continue;
                    }
                    var heroRes = HeroHelper.GetHeroResources(acc);

                    if (ResourcesHelper.IsEnoughRes(heroRes, stillNeededRes))
                    {
                        // If we have enough hero res for our task, execute the task
                        // right after hero equip finishes
                        acc.Logger.Warning($"Not enough resources to construct {_buildingTask.Building} - Level {_buildingTask.Level}! Needed {cost}. Bot will use resource from hero inventory", this);

                        var heroEquipTask = ResourcesHelper.UseHeroResources(acc, Vill, ref stillNeededRes, heroRes, _buildingTask);
                        await heroEquipTask.Execute(acc);
                        continue;
                    }
                }

                bool result;
                if (construct)
                {
                    result = await Construct(acc, contractNode);
                }
                else
                {
                    result = await Upgrade(acc, contractNode);
                }
                if (!result)
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
                else
                {
                    await DriverHelper.WaitPageChange(acc, "dorf");
                }
                PostTaskCheckDorf(acc);
            }
            while (true);
        }

        /// <summary>
        /// Building isn't constructed yet - construct it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskResult</returnss>
        private async Task<bool> Construct(Account acc, HtmlNode node)
        {
            var button = node.Descendants("button").FirstOrDefault(x => x.HasClass("new"));

            // Check for prerequisites
            if (button == null)
            {
                return false;
            }

            await DriverHelper.ClickById(acc, button.Id);

            _buildingTask.ConstructNew = false;

            acc.Logger.Warning($"Started construction of {_buildingTask.Building} in {Vill.Name}");
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
        private async Task<bool> Upgrade(Account acc, HtmlNode node)
        {
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
                acc.Logger.Warning($"We wanted to upgrade {_buildingTask.Building}, but no 'upgrade' button was found! Url={acc.Wb.CurrentUrl}");
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
                acc.Logger.Warning($"We wanted to upgrade {_buildingTask.Building}, but no 'upgrade' button was found!");
                return false;
            }

            // Not enough resources?
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5 && errorMessage != null)
            {
                acc.Logger.Warning($"We wanted to upgrade {_buildingTask.Building}, but there was an error message:\n{errorMessage.InnerText}");
                return false;
            }

            var buildDuration = InfrastructureParser.GetBuildDuration(container, acc.AccInfo.ServerVersion);

            acc.Logger.Information($"Started upgrading {_buildingTask.Building} to level {lvl + 1} in {Vill.Name}");

            var watchAd = false;
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5 && buildDuration.TotalMinutes > acc.Settings.WatchAdAbove)
            {
                watchAd = await TryFastUpgrade(acc);
            }

            if (!watchAd)
            {
                await DriverHelper.ClickById(acc, upgradeButton.Id); // Normal upgrade
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
            if (!await DriverHelper.ClickByClassName(acc, "videoFeatureButton green", log: false)) return false;
            await Task.Delay(AccountHelper.Delay(acc));

            // Confirm
            acc.Wb.UpdateHtml();
            if (acc.Wb.Html.DocumentNode.SelectSingleNode("//input[@name='adSalesVideoInfoScreen']") != null)
            {
                await DriverHelper.ClickByName(acc, "adSalesVideoInfoScreen");
                await Task.Delay(AccountHelper.Delay(acc));

                await DriverHelper.ExecuteScript(acc, "jQuery(window).trigger('showVideoWindowAfterInfoScreen')");
                await Task.Delay(AccountHelper.Delay(acc));
            }

            while (true)
            {
                acc.Wb.UpdateHtml();
                var node = acc.Wb.Html.GetElementbyId("videoFeature");
                if (node == null) continue;
                var element = acc.Wb.Driver.FindElementById("videoFeature");
                if (element == null) continue;
                element.Click();
                break;
            }
            // Has to be a legit "click"

            // wait for finish watching ads
            var timeout = DateTime.Now.AddSeconds(100);
            do
            {
                await Task.Delay(3000);

                //skip ads from Travian Games
                //they use ifarme to emebed ads video to their game
                acc.Wb.UpdateHtml();

                if (acc.Wb.Html.GetElementbyId("videoArea") != null)
                {
                    acc.Wb.SwitchTo().Frame(acc.Wb.FindElementById("videoArea"));

                    // trick to skip
                    await DriverHelper.ExecuteScript(acc, "var video = document.getElementsByTagName('video')[0];video.currentTime = video.duration - 1;", false, false);
                    //back to first page

                    acc.Wb.SwitchTo().DefaultContent();
                }
                if (timeout < DateTime.Now) return false;
            }
            while (acc.Wb.CurrentUrl.Contains("build.php"));

            // Don't show again
            await Task.Delay(1000);
            acc.Wb.UpdateHtml();

            if (acc.Wb.Html.GetElementbyId("dontShowThisAgain") != null)
            {
                await DriverHelper.ClickById(acc, "dontShowThisAgain");
                await Task.Delay(800);
                await DriverHelper.ClickByClassName(acc, "dialogButtonOk ok");
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
                acc.Logger.Information($"Started (fast) upgrading {building.Type} to level {lvl} in {this.Vill?.Name}", this);

                var build = acc.Wb.Html.GetElementbyId("build");
                if (build != null) RemoveCurrentTask(); // Already on max lvl
                else PostTaskCheckDorf(acc);
                return true;
            }

            return false;
        }

        private bool EnoughFreeCrop(Account acc)
        {
            // 5 is maximum a building can take up free crop (stable lvl 1)
            if (Vill.Res.FreeCrop <= 5 && _buildingTask.Building != BuildingEnum.Cropland)
            {
                UpgradeBuildingHelper.UpgradeBuildingForOneLvl(acc, Vill, BuildingEnum.Cropland, false);
                return false;
            }
            return true;
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

                UpgradeBuildingHelper.AddBuildingTask(acc, vill, bonusTask, false, restart);
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

        /// <summary>
        /// Helper method for checking whether the bot should add the bonus building to the build list
        /// </summary>
        private bool BonusHelper(Village vill, BuildingEnum field, BuildingEnum bonus, int fieldLvl)
        {
            // If the bonus building is currently being upgraded to level 5, don't try to re-add it
            if (vill.Build.CurrentlyBuilding.Any(x => x.Building == bonus && x.Level == 5)) return false;

            // Bonus building is not on 5, res field is high enough, there is still space left to build, there isn't already a bonus building buildtask
            return (!vill.Build.Buildings.Any(x => x.Type == bonus && 5 <= x.Level) &&
                vill.Build.Buildings.Any(x => x.Type == field && fieldLvl <= x.Level) &&
                vill.Build.Buildings.Any(x => x.Type == BuildingEnum.Site) &&
                !vill.Build.Tasks.Any(x => x.Building == bonus));
        }

        /// <summary>
        /// Upgrades specified building for exactly one level. Will upgrade the lowest level building.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="building">Building to be upgraded by one</param>
        /// <param name="bottom">Whether to insert the building task on the bottom of the build list</param>
        /// <returns>Whether the method executed successfully</returns>
        internal static bool UpgradeBuildingForOneLvl(Account acc, Village vill, BuildingEnum building, bool bottom = true)
        {
            // We already have a build task
            if (!bottom && vill.Build.Tasks.FirstOrDefault()?.Building == building) return true;
            if (bottom && vill.Build.Tasks.LastOrDefault()?.Building == building) return true;

            var upgrade = vill.Build
                .Buildings
                .OrderBy(x => x.Level)
                .FirstOrDefault(x => x.Type == building);

            // We don't have this building in the village yet
            if (upgrade == null)
            {
                return UpgradeBuildingHelper.AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = building,
                    Level = 1,
                }, bottom);
            }

            // Current lvl in bot's data
            var currentLvl = (int)upgrade.Level;

            UpgradeBuildingHelper.RemoveFinishedCB(vill);

            // Current lvl in current building list
            var currentBuilding = vill.Build.CurrentlyBuilding.FirstOrDefault(x => x.Building == building);
            if (currentBuilding != null) currentLvl = currentBuilding.Level;

            if (BuildingsData.MaxBuildingLevel(acc, building) == currentLvl)
            {
                // Building is on max level, construct new building if possible
                if (!BuildingsData.CanHaveMultipleBuildings(building)) return false;

                return UpgradeBuildingHelper.AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = building,
                    Level = 1,
                }, bottom);
            }
            else // Upgrade the defined building
            {
                return UpgradeBuildingHelper.AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = BuildingType.General,
                    Building = building,
                    Level = currentLvl + 1,
                    BuildingId = upgrade.Id
                }, bottom);
            }
        }

        private async Task SwitchVillage(Account acc)
        {
            await DriverHelper.WaitPageLoaded(acc);
            var active = acc.Villages.FirstOrDefault(x => x.Active);
            if (active != null && active.Id != Vill.Id)
            {
                await VillageHelper.SwitchVillage(acc, Vill.Id);
            }
        }
    }
}