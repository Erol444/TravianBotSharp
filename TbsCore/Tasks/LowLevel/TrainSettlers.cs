using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TrainSettlers : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Residence || x.Type == Classificator.BuildingEnum.Palace || x.Type == Classificator.BuildingEnum.CommandCenter);
            if (building == null)
            {
                //update dorg, no buildingId found?
                TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = DateTime.Now, Vill = Vill });
                Console.WriteLine($"There is no Residence/Palace/CommandCenter in this village!");
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?s=1&id={building.Id}");

            var cost = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            if (cost == null)
            {
                this.ErrorMessage = "Could not train settlers. Will retry in 5min";
                this.NextExecute = DateTime.Now.AddMinutes(5); //retry in 5min
                return TaskRes.Executed;
            }

            var resources = ResourceParser.GetResourceCost(cost);
            var enoughResAt = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, resources);
            if (enoughResAt <= DateTime.Now.AddMilliseconds(1)) //we have enough res, create new settler!
            {
                wb.ExecuteScript($"document.getElementsByName('t10')[0].value='1'");
                await Task.Delay(AccountHelper.Delay());
                wb.ExecuteScript($"document.getElementById('s1').click()"); //Train settler
                Vill.Troops.Settlers++;
                if (Vill.Troops.Settlers < 3)
                {
                    //In 1 minute, do the same task (to get total of 3 settlers)
                    this.NextExecute = DateTime.Now.AddSeconds(1);
                }
                else
                {
                    if (acc.NewVillages.AutoSettleNewVillages)
                    {
                        //parse in training table
                        this.PostTaskCheck.Add(NewVillage);
                    }
                }
                return TaskRes.Executed;
            }
            else
            {
                //Not enough res, wait for production/transit res
                this.NextExecute = enoughResAt.AddSeconds(60 * AccountHelper.Delay());
                return TaskRes.Executed;
            }
        }
        public void NewVillage(HtmlDocument htmlDoc, Account acc)
        {
            //TODO: parse when the 3rd settler will be trained.
            var training = TroopsHelper.TrainingDuration(htmlDoc);
            TaskExecutor.AddTaskIfNotExists(acc, new SendSettlers() {
                ExecuteAt = training.AddSeconds(3),
                Vill = this.Vill,
                Priority = TaskPriority.Medium
            });
        }
    }
}
