using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;
using TbsCore.TravianData;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.Sim
{
    public class TrainSettlers : BotTask
    {
        private TroopsEnum settlerId;

        public override async Task<TaskRes> Execute(Account acc)
        {
            settlerId = TroopsData.TribeSettler(acc.AccInfo.Tribe);
            StopFlag = false;
            do
            {
                if (StopFlag) return TaskRes.Executed;

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Checking building ...", this);
                    var result = await BuildingRequired(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Enter building ...", this);
                    var result = await EnterBuilding(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Checking settlers condition ...", this);
                    var result = EnoughSettlers(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Checking settler current number ...", this);
                    var result = UpdateSettlersAmount(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Checking resource ...", this);
                    var result = await IsEnoughRes(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Enter amount ... ", this);
                    await DriverHelper.WriteByName(acc, "t10", 1);
                    acc.Logger.Information("Click train button ... ", this);
                    await Task.Delay(900);
                    await DriverHelper.ClickById(acc, "s1");
                }
            }
            while (true);
        }

        private void SendSettlersTask(Account acc)
        {
            acc.Wb.UpdateHtml();
            var training = TroopsHelper.TrainingDuration(acc.Wb.Html);
            if (training < DateTime.Now) training = DateTime.Now;
            training = training.AddSeconds(5);

            acc.Logger.Information($"Bot will (try to) send settlers in {TimeHelper.InSeconds(training)} sec");

            acc.Tasks.Add(new SendSettlers()
            {
                ExecuteAt = training,
                Vill = this.Vill,
                // For high speed servers, you want to train settlers asap
                Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
            }, true);
        }

        private async Task<bool> BuildingRequired(Account acc)
        {
            var building = Vill.Build.Buildings.FirstOrDefault(x =>
                x.Type == BuildingEnum.Residence ||
                x.Type == BuildingEnum.Palace ||
                x.Type == BuildingEnum.CommandCenter
            );
            if (building == null)
            {
                acc.Logger.Information($"Cannot found residence/palace or command center in {Vill.Name}");
                acc.Logger.Information($"Update dorf2 to confirm");
                {
                    var resultSwitch = await NavigationHelper.SwitchVillage(acc, Vill);
                    if (!resultSwitch)
                    {
                        Retry(acc, "Cannot switch village");
                        return false;
                    }
                }
                {
                    var resultSwitch = await NavigationHelper.ToDorf2(acc);
                    if (!resultSwitch)
                    {
                        Retry(acc, "Cannot enter dorf2");
                        return false;
                    }
                }

                building = Vill.Build.Buildings.FirstOrDefault(x =>
                    x.Type == BuildingEnum.Residence ||
                    x.Type == BuildingEnum.Palace ||
                    x.Type == BuildingEnum.CommandCenter
                );

                if (building == null)
                {
                    acc.Logger.Information($"Confirm there isn't residence/palace or command center in {Vill.Name}");
                    StopFlag = true;
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> EnterBuilding(Account acc)
        {
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (!result) return false;
            }
            return await NavigationHelper.ToGovernmentBuilding(acc, Vill, NavigationHelper.ResidenceTab.Train);
        }

        private bool EnoughSettlers(Account acc)
        {
            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass($"u{(int)settlerId}"));

            if (troopNode == null)
            {
                acc.Logger.Information("No new settler can be trained, probably because 3 settlers are already (being) trained");
                SendSettlersTask(acc);
                StopFlag = true;
                return false;
            }
            return true;
        }

        private bool UpdateSettlersAmount(Account acc)
        {
            HtmlAgilityPack.HtmlNode nodeSettler = null;
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.TTwars:
                    {
                        var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)settlerId));
                        while (!troopNode.HasClass("details"))
                        {
                            troopNode = troopNode.ParentNode;
                            if (troopNode == null)
                            {
                                acc.Logger.Information("No new settler can be trained, probably because 3 settlers are already (being) trained");
                                SendSettlersTask(acc);
                                StopFlag = true;
                                return false;
                            }
                        }
                        nodeSettler = troopNode;
                    }
                    break;

                case ServerVersionEnum.T4_5:
                    {
                        var troopBox = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass($"troop{(int)settlerId}") && x.HasClass("innerTroopWrapper"));
                        if (troopBox == null)
                        {
                            Retry(acc, "Cannot find settler box");
                            return false;
                        }
                        nodeSettler = troopBox;
                    }
                    break;

                default:
                    break;
            }
            var divTit = nodeSettler.Descendants("div").FirstOrDefault(x => x.HasClass("tit"));
            if (divTit == null)
            {
                Retry(acc, "Cannot find Settler title");
                return false;
            }
            var spanPresent = divTit.Descendants("span").FirstOrDefault();
            if (spanPresent == null)
            {
                Retry(acc, "Cannot find Settler present number");
                return false;
            }

            Vill.Troops.Settlers = (int)Parser.RemoveNonNumeric(spanPresent.InnerText);
            acc.Logger.Information($"Update Settler present number: {Vill.Troops.Settlers}");

            acc.Wb.UpdateHtml();
            var training = TroopsParser.GetTroopsCurrentlyTraining(acc.Wb.Html);
            var countTraning = 0;
            foreach (var item in training)
            {
                if (item.Troop == settlerId) countTraning += item.TrainNumber;
            }
            acc.Logger.Information($"Update Settler training number: {countTraning}");

            if (Vill.Troops.Settlers + countTraning >= 3)
            {
                acc.Logger.Information($"Have enough settler.");
                StopFlag = true;
                if (acc.NewVillages.AutoSettleNewVillages)
                {
                    SendSettlersTask(acc);
                }
                return false;
            }
            return true;
        }

        private async Task<bool> IsEnoughRes(Account acc)
        {
            HtmlNode troopBox = null;
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.TTwars:
                    var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)settlerId));
                    while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;
                    troopBox = troopNode;
                    break;

                case ServerVersionEnum.T4_5:
                    troopBox = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass($"troop{(int)settlerId}") && x.HasClass("innerTroopWrapper"));
                    if (troopBox == null)
                    {
                        Retry(acc, "Cannot find settler box");
                        return false;
                    }
                    break;
            }

            var resWrapper = troopBox.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            if (resWrapper == null)
            {
                Retry(acc, "Cannot find resource list");
                return false;
            }
            var cost = ResourceParser.GetResourceCost(resWrapper);

            acc.Logger.Information($"Need {cost}");

            if (!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
            {
                if (ResourcesHelper.IsStorageTooLow(acc, Vill, cost))
                {
                    acc.Logger.Warning($"Storage is too low. Added storage upgrade.");
                    StopFlag = true;
                    return false;
                }

                var stillNeededRes = ResourcesHelper.SubtractResources(cost.ToArray(), Vill.Res.Stored.Resources.ToArray(), true);
                acc.Logger.Information("Not enough resources to train.");
                if (Vill.Settings.UseHeroRes && acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5) // Only T4.5 has resources in hero inv
                {
                    var heroRes = HeroHelper.GetHeroResources(acc);

                    if (ResourcesHelper.IsEnoughRes(heroRes, stillNeededRes))
                    {
                        // If we have enough hero res for our task, execute the task
                        // right after hero equip finishes
                        acc.Logger.Information("Bot will use resource from hero inventory");

                        var heroEquipTask = ResourcesHelper.UseHeroResources(acc, Vill, ref stillNeededRes, heroRes);
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