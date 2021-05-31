﻿using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Parsers;

using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Old train troops task, only here because of high speed TTWars servers.
    /// </summary>
    public class TrainTroops : BotTask
    {
        /// <summary>
        /// Great barracks/stable?
        /// </summary>
        public bool Great { get; set; }

        /// <summary>
        /// Which troop we want to train
        /// </summary>
        public TroopsEnum Troop { get; set; }

        /// <summary>
        /// If true, will just check CurrentlyTraining and Researched.
        /// </summary>
        public bool UpdateOnly { get; set; }

        /// <summary>
        /// If we play on UNL/VIP, don't repeat this cycle; this tasks gets called every time after FL/buying res
        /// </summary>
        public bool HighSpeedServer { get; set; }

        private BuildingEnum building;

        public override async Task<TaskRes> Execute(Account acc)
        {
            building = TroopsHelper.GetTroopBuilding(Troop, Great);

            // Switch hero helmet. If hero will be switched, this TrainTroops task
            // will be executed right after the hero helmet switch
            if (HeroHelper.SwitchHelmet(acc, this.Vill, building, this)) return TaskRes.Executed;

            if (!await VillageHelper.EnterBuilding(acc, Vill, building))
                return TaskRes.Executed;

            if (this.UpdateOnly || this.Troop == TroopsEnum.None)
            {
                return TaskRes.Executed;
            }

            (TimeSpan dur, Resources cost) = TroopsParser.GetTrainCost(acc.Wb.Html, this.Troop);

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)Troop));

            if (troopNode == null)
            {
                acc.Logger.Warning($"Bot tried to train {Troop} in {Vill.Name}, but couldn't find it in {building}! Are you sure you have {Troop} researched?");
                return TaskRes.Executed;
            }
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;
            var inputName = troopNode.Descendants("input").FirstOrDefault().GetAttributeValue("name", "");

            long maxNum = 0;
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.T4_4:
                    maxNum = Parser.RemoveNonNumeric(
                        troopNode.ChildNodes
                        .FirstOrDefault(x => x.Name == "a")?.InnerText ?? "0"
                        );
                    break;

                case ServerVersionEnum.T4_5:
                    maxNum = Parser.RemoveNonNumeric(
                            troopNode.ChildNodes
                            .First(x => x.HasClass("cta"))
                            .ChildNodes
                            .First(x => x.Name == "a")
                            .InnerText);
                    break;
            }

            if (!HighSpeedServer)
            {
                var trainNum = TroopsHelper.TroopsToFill(acc, Vill, this.Troop, this.Great);

                // Don't train too many troops, just fill up the training building
                if (maxNum > trainNum) maxNum = trainNum;
            }

            if (maxNum < 0)
            {
                // We have already enough troops in training.
                return TaskRes.Executed;
            }

            acc.Wb.Driver.ExecuteScript($"document.getElementsByName('{inputName}')[0].value='{maxNum}'");

            await Task.Delay(100);

            await DriverHelper.ExecuteScript(acc, "document.getElementsByName('s1')[0].click()");
            UpdateCurrentlyTraining(acc.Wb.Html, acc);

            if (!HighSpeedServer) RepeatTrainingCycle(acc.Wb.Html, acc);

            return TaskRes.Executed;
        }

        public void UpdateCurrentlyTraining(HtmlDocument htmlDoc, Account acc)
        {
            var ct = TroopsParser.GetTroopsCurrentlyTraining(htmlDoc);
            switch (building)
            {
                case Classificator.BuildingEnum.Barracks:
                    Vill.Troops.CurrentlyTraining.Barracks = ct;
                    break;

                case Classificator.BuildingEnum.Stable:
                    Vill.Troops.CurrentlyTraining.Stable = ct;
                    break;

                case Classificator.BuildingEnum.GreatBarracks:
                    Vill.Troops.CurrentlyTraining.GB = ct;
                    break;

                case Classificator.BuildingEnum.GreatStable:
                    Vill.Troops.CurrentlyTraining.GS = ct;
                    break;

                case Classificator.BuildingEnum.Workshop:
                    Vill.Troops.CurrentlyTraining.Workshop = ct;
                    break;
            }
        }

        /// <summary>
        /// Repeats sending resources and training troops. Needs to fill up training above X hours.
        /// </summary>
        /// <param name="htmlDoc">html of the page</param>
        /// <param name="acc">Account</param>
        public void RepeatTrainingCycle(HtmlDocument htmlDoc, Account acc)
        {
            var trainingEnds = TroopsHelper.GetTrainingTimeForBuilding(building, Vill);

            // If sendRes is activated and there are some resources left to send
            if (Vill.Settings.SendRes && 0 < MarketHelper.GetResToMainVillage(this.Vill).Sum())
            {
                // Check If all troops are filled in this vill before sending resources back to main village
                if (TroopsHelper.EverythingFilled(acc, Vill))
                {
                    acc.Tasks.Add(new SendResToMain() { Vill = this.Vill, ExecuteAt = DateTime.MinValue.AddHours(1) });
                }
            }

            var mainVill = AccountHelper.GetMainVillage(acc);
            if (Vill.Settings.GetRes && mainVill != this.Vill)
            {
                var nextCycle = trainingEnds.AddHours(-acc.Settings.FillInAdvance);

                if (nextCycle < Vill.Market.LastTransit.AddMinutes(5))
                    nextCycle = Vill.Market.LastTransit.AddMinutes(5);

                if (nextCycle < DateTime.Now)
                {
                    // Send resources asap.
                    nextCycle = DateTime.MinValue.AddHours(3);
                }
                else
                {
                    acc.Tasks.Add(new UpdateDorf1()
                    {
                        ExecuteAt = nextCycle,
                        Vill = this.Vill
                    });
                }

                acc.Tasks.Add(new SendResFillTroops()
                {
                    ExecuteAt = nextCycle.AddMilliseconds(1),
                    Vill = mainVill,
                    TargetVill = this.Vill,
                    TrainTask = this
                });
                this.NextExecute = nextCycle.AddMinutes(30); //will get overwritten in sendResFillTroops
                acc.Tasks.ReOrder();
            }
            else
            {
                var subtractMillis = AccountHelper.Delay() * 50; //~30sec
                var later = DateTime.Now.AddMinutes(10);
                // Don't training again sooner than after 10min
                if (later > trainingEnds) trainingEnds = later;

                this.NextExecute = trainingEnds.AddMilliseconds(-subtractMillis);
            }
        }
    }
}