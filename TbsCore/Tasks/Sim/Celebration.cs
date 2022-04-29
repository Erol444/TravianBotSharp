using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;
using TbsCore.TravianData;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.Sim
{
    public class Celebration : BotTask
    {
        private bool bigCeleb;
        private readonly Random rand = new Random();

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
                    acc.Logger.Information("Moving into TownHall ...", this);
                    var result = await MoveIntoBuilding(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Check current celebration  ...");
                    var result = IsFreeSlot(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Check enough resource ...", this);
                    var result = IsEnoughRes(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Click start celebration ...", this);
                    var result = await StartCelebration(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    acc.Logger.Information("Post check ...", this);
                    var result = PostCheck(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) continue;
                }
            } while (true);
        }

        private async Task<bool> MoveIntoBuilding(Account acc)
        {
            acc.Logger.Information($"Checking current village ...");
            {
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (!result)
                {
                    Retry(acc, "Cannot switch village");
                    return false;
                }
            }

            await AccountHelper.DelayWait(acc);

            {
                var result = await NavigationHelper.EnterBuilding(acc, Vill, BuildingEnum.TownHall);
                if (!result)
                {
                    Retry(acc, "Cannot enter TownHall");
                    return false;
                }
            }
            return true;
        }

        private bool IsFreeSlot(Account acc)
        {
            HtmlNode underProgressNode;
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.TTwars:
                    underProgressNode = acc.Wb.Html.GetElementbyId("under_progress");
                    break;

                case ServerVersionEnum.T4_5:
                    {
                        var content = acc.Wb.Html.GetElementbyId("content");
                        underProgressNode = content.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
                    }
                    break;

                default:
                    underProgressNode = null;
                    break;
            }

            if (underProgressNode == null) return true; // No celebration is under progress

            var celebrationEnd = DateTime.Now + TimeParser.ParseTimer(underProgressNode);
            Vill.Expansion.CelebrationEnd = celebrationEnd;
            NextExecute = celebrationEnd;
            acc.Logger.Information($"There is under progress celebration. [{celebrationEnd}]");
            StopFlag = true;
            return false;
        }

        private bool IsEnoughRes(Account acc)
        {
            var townHall = Vill.Build.Buildings.FirstOrDefault(x => x.Type == BuildingEnum.TownHall);
            if (townHall == null)
            {
                Retry(acc, "Townhall disappear from database");
                return false;
            }

            bigCeleb = Vill.Expansion.Celebrations == CelebrationEnum.Big && 10 <= townHall.Level;
            var arrayCost = MiscCost.CelebrationCost(bigCeleb);
            var cost = ResourcesHelper.ArrayToResources(arrayCost);
            acc.Logger.Information($"Need {cost}");
            if (ResourcesHelper.IsEnoughRes(Vill, arrayCost))
            {
                return true;
            }

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
                }
            }
            else
            {
                var stillNeededRes = ResourcesHelper.SubtractResources(cost.ToArray(), Vill.Res.Stored.Resources.ToArray(), true);
                acc.Logger.Information($"Not enough resources to build. Still need {stillNeededRes}");
                acc.Logger.Information($"Bot will try finish the task later");
                DateTime enoughRes = TimeHelper.EnoughResToUpgrade(Vill, stillNeededRes);
                NextExecute = TimeHelper.RanDelay(acc, enoughRes);
            }

            StopFlag = true;
            return false;
        }

        private async Task<bool> StartCelebration(Account acc)
        {
            var nodes = acc.Wb.Html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));

            var node = bigCeleb ? nodes.LastOrDefault() : nodes.FirstOrDefault();
            if (node == null)
            {
                Retry(acc, "Cannot find button box");
                return false;
            }

            var button = node.Descendants("button").FirstOrDefault();
            if (node == null)
            {
                Retry(acc, "Cannot find button");
                return false;
            }

            var elementButton = acc.Wb.Driver.FindElement(By.XPath(button.XPath));
            elementButton.Click();
            await Task.Delay(rand.Next(1200, 2000));
            return true;
        }

        private bool PostCheck(Account acc)
        {
            HtmlNode underProgressNode;
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.TTwars:
                    underProgressNode = acc.Wb.Html.GetElementbyId("under_progress");
                    break;

                case ServerVersionEnum.T4_5:
                    {
                        var content = acc.Wb.Html.GetElementbyId("content");
                        underProgressNode = content.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
                    }
                    break;

                default:
                    underProgressNode = null;
                    break;
            }

            if (underProgressNode == null) return true; // No celebration is under progress

            var celebrationEnd = DateTime.Now + TimeParser.ParseTimer(underProgressNode);
            Vill.Expansion.CelebrationEnd = celebrationEnd;
            NextExecute = celebrationEnd;
            StopFlag = true;
            return false;
        }
    }
}