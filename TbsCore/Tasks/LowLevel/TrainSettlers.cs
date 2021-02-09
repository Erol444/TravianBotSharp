using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TrainSettlers : UpdateDorf2
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await base.Execute(acc); // Navigate to dorf2

            var building = Vill.Build.Buildings
                .FirstOrDefault(x => 
                    x.Type == Classificator.BuildingEnum.Residence ||
                    x.Type == Classificator.BuildingEnum.Palace ||
                    x.Type == Classificator.BuildingEnum.CommandCenter
                );

            if (building == null)
            {
                acc.Wb.Log($"Can't train settlers, there is no Residence/Palace/CommandCenter in this village!");
                return TaskRes.Executed;
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?s=1&id={building.Id}");

            var settler = TroopsHelper.TribeSettler(acc.AccInfo.Tribe);
            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)settler));

            if (troopNode == null)
            {
                acc.Wb.Log("No new settler can be trained, probably because 3 settlers are already (being) trained");
                SendSettlersTask(acc);
                return TaskRes.Executed;
            }

            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            var maxNum = Parser.RemoveNonNumeric(troopNode.ChildNodes.First(x => x.Name == "a").InnerText);
            var available = TroopsParser.ParseAvailable(troopNode);

            var costNode = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var cost = ResourceParser.GetResourceCost(costNode);

            if(!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
            {
                ResourcesHelper.NotEnoughRes(acc, Vill, cost, this);
                return TaskRes.Executed;
            }

            acc.Wb.Driver.ExecuteScript($"document.getElementsByName('t10')[0].value='{maxNum}'");
            await Task.Delay(AccountHelper.Delay());
            // Click Train button
            await TbsCore.Helpers.DriverHelper.ExecuteScript(acc, "document.getElementById('s1').click()");
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

        private void SendSettlersTask(Account acc)
        {
            var training = TroopsHelper.TrainingDuration(acc.Wb.Html);
            if (training < DateTime.Now) training = DateTime.Now;
            training = training.AddSeconds(5);

            acc.Wb.Log($"Bot will (try to) send settlers in {TimeHelper.InSeconds(training)} sec");

            TaskExecutor.AddTaskIfNotExists(acc, new SendSettlers()
            {
                ExecuteAt = training,
                Vill = this.Vill,
                // For high speed servers, you want to train settlers asap
                Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
            });
        }
    }
}
