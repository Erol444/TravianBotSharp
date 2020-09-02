using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Parsers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
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

        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            building = TroopsHelper.GetTroopBuilding(Troop, Great);

            var buildId = vill.Build.Buildings.FirstOrDefault(x => x.Type == building);
            if (buildId == null)
            {
                //update dorf, no buildingId found?
                TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = DateTime.Now, vill = vill });
                Console.WriteLine($"There is no {building} in this village!");
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={buildId.Id}");

            //after finishing task, update currently training
            this.PostTaskCheck.Add(UpdateCurrentlyTraining);
            if (!HighSpeedServer) this.PostTaskCheck.Add(RepeatTrainingCycle);
            if (this.UpdateOnly || this.Troop == TroopsEnum.None)
            {
                return TaskRes.Executed;
            }

            (TimeSpan dur, Resources cost) = TroopsParser.GetTrainCost(htmlDoc, this.Troop);

            var troopNode = htmlDoc.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)Troop));
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;
            var inputName = troopNode.Descendants("input").FirstOrDefault().GetAttributeValue("name", "");

            var maxNum = Parser.RemoveNonNumeric(troopNode.ChildNodes.FirstOrDefault(x => x.Name == "a").InnerText);

            if (!HighSpeedServer)
            {
                var trainNum = TroopsHelper.TroopsToFill(acc, vill, this.Troop, this.Great);

                // Don't train too many troops, just fill up the training building
                if (maxNum > trainNum) maxNum = trainNum;
            }

            if (maxNum < 0)
            {
                // We have already enough troops in training.
                return TaskRes.Executed;
            }

            wb.ExecuteScript($"document.getElementsByName('{inputName}')[0].value='{maxNum}'");
            await Task.Delay(100);

            wb.ExecuteScript("document.getElementsByName('s1')[0].click()"); //Train button
            return TaskRes.Executed;
        }
        public void UpdateCurrentlyTraining(HtmlDocument htmlDoc, Account acc)
        {
            var ct = TroopsParser.GetTroopsCurrentlyTraining(htmlDoc);
            switch (building)
            {
                case Classificator.BuildingEnum.Barracks:
                    vill.Troops.CurrentlyTraining.Barracks = ct;
                    break;
                case Classificator.BuildingEnum.Stable:
                    vill.Troops.CurrentlyTraining.Stable = ct;
                    break;
                case Classificator.BuildingEnum.GreatBarracks:
                    vill.Troops.CurrentlyTraining.GB = ct;
                    break;
                case Classificator.BuildingEnum.GreatStable:
                    vill.Troops.CurrentlyTraining.GS = ct;
                    break;
                case Classificator.BuildingEnum.Workshop:
                    vill.Troops.CurrentlyTraining.Workshop = ct;
                    break;
            }
        }
        /// <summary>
        /// PostTask. Repeats sending resources and training troops. Needs to fill up training above X hours.
        /// </summary>
        /// <param name="htmlDoc">html of the page</param>
        /// <param name="acc">Account</param>
        public void RepeatTrainingCycle(HtmlDocument htmlDoc, Account acc)
        {
            var trainingEnds = TroopsHelper.GetTrainingTimeForBuilding(building, vill);

            // If sendRes is activated and there are some resources left to send
            if (vill.Settings.SendRes && MarketHelper.GetResToMainVillage(this.vill).Sum() > 0)
            {
                // Check If all troops are filled in this vill before sending resources back to main village
                if (TroopsHelper.EverythingFilled(acc, vill))
                {
                    TaskExecutor.AddTask(acc, new SendResToMain() { vill = this.vill, ExecuteAt = DateTime.MinValue.AddHours(1) });
                }
            }

            var mainVill = AccountHelper.GetMainVillage(acc);
            if (vill.Settings.GetRes && mainVill != this.vill)
            {
                var nextCycle = trainingEnds.AddHours(-acc.Settings.FillInAdvance);
                if (nextCycle < DateTime.Now)
                {
                    // Send resources asap.
                    nextCycle = DateTime.MinValue.AddHours(3);
                }
                else
                {
                    TaskExecutor.AddTask(acc, new UpdateDorf1()
                    {
                        ExecuteAt = nextCycle,
                        vill = this.vill
                    });
                }

                TaskExecutor.AddTask(acc, new SendResFillTroops()
                {
                    ExecuteAt = nextCycle.AddMilliseconds(1),
                    vill = mainVill,
                    TargetVill = this.vill,
                    TrainTask = this
                });
                this.NextExecute = nextCycle.AddMinutes(30); //will get overwritten in sendResFillTroops
                TaskExecutor.ReorderTaskList(acc);
            }
            else
            {
                var later = DateTime.Now.AddMinutes(10);
                // Don't training again sooner than after 10min
                if (later > trainingEnds) trainingEnds = later;

                this.NextExecute = trainingEnds;
            }
        }
    }
}