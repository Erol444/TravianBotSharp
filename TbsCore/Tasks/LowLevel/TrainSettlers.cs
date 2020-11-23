using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TrainSettlers : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Residence || x.Type == Classificator.BuildingEnum.Palace || x.Type == Classificator.BuildingEnum.CommandCenter);
            if (building == null)
            {
                //update dorg, no buildingId found?
                TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = DateTime.Now, Vill = Vill });
                acc.Wb.Log($"There is no Residence/Palace/CommandCenter in this village!");
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?s=1&id={building.Id}");

            var settler = TroopsHelper.TribeSettler(acc.AccInfo.Tribe);
            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)settler));

            if (troopNode == null)
            {
                // No settler can be trained.
                SendSettlersTask(acc);
                return TaskRes.Executed;
            }
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;
            //var inputName = troopNode.Descendants("input").FirstOrDefault().GetAttributeValue("name", "");

            var maxNum = Parser.RemoveNonNumeric(troopNode.ChildNodes.First(x => x.Name == "a").InnerText);
            var available = TroopsParser.ParseAvailable(troopNode);

            var cost = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));

            var resources = ResourceParser.GetResourceCost(cost);
            var enoughResAt = ResourcesHelper.EnoughResourcesOrTransit(acc, Vill, resources);
            if (enoughResAt <= DateTime.Now.AddMilliseconds(1)) //we have enough res, create new settler!
            {
                await acc.Wb.Driver.FindElementByName("t10").Write(maxNum);
                
                // Click Train button
                await acc.Wb.Driver.FindElementById("s1").Click(acc);
                
                Vill.Troops.Settlers = (int)available + (int)maxNum;

                var training = TroopsHelper.TrainingDuration(acc.Wb.Html);
                if (training < DateTime.Now) training = DateTime.Now;

                if (Vill.Troops.Settlers < 3)
                {
                    //In 1 minute, do the same task (to get total of 3 settlers)
                    this.NextExecute = training.AddSeconds(3);
                }
                else
                {
                    if (acc.NewVillages.AutoSettleNewVillages)
                    {
                        SendSettlersTask(acc);
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
        private void SendSettlersTask(Account acc)
        {
            var training = TroopsHelper.TrainingDuration(acc.Wb.Html);
            if (training < DateTime.Now) training = DateTime.Now;
            TaskExecutor.AddTaskIfNotExists(acc, new SendSettlers()
            {
                ExecuteAt = training.AddSeconds(5),
                Vill = this.Vill
            });
        }
    }
}
