using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Models.ResourceModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;
using static TravBotSharp.Files.Helpers.Classificator;
using TravBotSharp.Files.Tasks.SecondLevel;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    ///
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
        public bool Repeat { get; set; } = false;

        private BuildingEnum building;

        public override async Task<TaskRes> Execute(Account acc)
        {
            building = TroopsHelper.GetTroopBuilding(Troop, Great);

            // Switch hero helmet. If hero will be switched, this TrainTroops task
            // will be executed right after the hero helmet switch
            if (HeroHelper.SwitchHelmet(acc, this.Vill, building, this)) return TaskRes.Executed;

            if (!await VillageHelper.EnterBuilding(acc, Vill, building))
                return TaskRes.Executed;

            var currentlyTrainings = UpdateCurrentlyTraining(acc.Wb.Html, acc);

            if (this.UpdateOnly || this.Troop == TroopsEnum.None)
            {
                return TaskRes.Executed;
            }

            // check current training before calc to train
            if (currentlyTrainings.Count > 0)
            {
                var finishTraining = currentlyTrainings.Last().FinishTraining;
                if ((finishTraining - DateTime.Now).TotalHours > acc.Settings.FillInAdvance)
                {
                    NextExecute = currentlyTrainings.Last().FinishTraining.AddHours(-acc.Settings.FillFor);
                    return TaskRes.Executed;
                }
            }

            (TimeSpan dur, Resources cost) = TroopsParser.GetTrainCost(acc.Wb.Html, this.Troop);

            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)Troop));

            if (troopNode == null)
            {
                acc.Wb.Log($"Bot tried to train {Troop} in {Vill.Name}, but couldn't find it in {building}! Are you sure you have {Troop} researched?");
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

            var trainNum = TroopsHelper.TroopsToFill(acc, Vill, Troop, Great);

            // Don't train too many troops, just fill up the training building
            if (maxNum > trainNum) maxNum = trainNum;

            if (maxNum < 0)
            {
                // We have already enough troops in training.
                return TaskRes.Executed;
            }

            // calculate how many resources we need to train trainNum of troops
            long[] neededRes = cost.ToArray().Select(x => x * trainNum).ToArray();

            // if we dont have enough resources in the target village, send res from main village
            // if current village is main, just train with current res noneed to wait
            if (!ResourcesHelper.IsEnoughRes(Vill, neededRes))
            {
                if (!Vill.Coordinates.Equals(AccountHelper.GetMainVillage(acc).Coordinates))
                {
                    TaskExecutor.AddTask(acc, new SendResFillTroops()
                    {
                        ExecuteAt = DateTime.Now,
                        Vill = AccountHelper.GetMainVillage(acc),
                        TargetVill = this.Vill,
                        Troop = Troop,
                        Great = Great
                    });
                }
                return TaskRes.Executed;
            }

            // train our troops
            if (maxNum > 0)
            {
                acc.Wb.Driver.ExecuteScript($"document.getElementsByName('{inputName}')[0].value='{maxNum}'");
                await Task.Delay(AccountHelper.Delay());
                await DriverHelper.ExecuteScript(acc, "document.getElementsByName('s1')[0].click()");
                await Task.Delay(AccountHelper.Delay() * 3);
                acc.Wb.UpdateHtml();
                currentlyTrainings = UpdateCurrentlyTraining(acc.Wb.Html, acc);
            }
            var ran = new Random();

            // make sefl update by getting last FinishTraining minus half acc.Settings.FillFor
            if (Repeat)
            {
                if (currentlyTrainings.Count > 0)
                {
                    var TimeShoulTrainNext = currentlyTrainings.Last().FinishTraining.AddHours(-acc.Settings.FillFor);
                    // TimeShouldTrain is later than now
                    if (DateTime.Compare(TimeShoulTrainNext, DateTime.Now) > 0)
                    {
                        NextExecute = TimeShoulTrainNext.AddMinutes(-(ran.Next(1, 5)));
                    }
                    else
                    {
                        NextExecute = DateTime.Now.AddMinutes(ran.Next(30, 60));
                    }
                }
                else
                {
                    NextExecute = DateTime.Now.AddMinutes(ran.Next(30, 60));
                }
            }

            return TaskRes.Executed;
        }

        public List<TroopsCurrentlyTraining> UpdateCurrentlyTraining(HtmlDocument htmlDoc, Account acc)
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

            return ct;
        }
    }
}