using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.TravianData;
using TbsCore.Helpers;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
{
    public class TrainSettlers : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var building = Vill.Build.Buildings
                .FirstOrDefault(x =>
                    x.Type == Classificator.BuildingEnum.Residence ||
                    x.Type == Classificator.BuildingEnum.Palace ||
                    x.Type == Classificator.BuildingEnum.CommandCenter
                );

            if (!await VillageHelper.EnterBuilding(acc, building, "&s=1"))
                return TaskRes.Executed;

            var settler = TroopsData.TribeSettler(acc.AccInfo.Tribe);
            var troopNode = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)settler));

            if (troopNode == null)
            {
                acc.Logger.Warning("No new settler can be trained, probably because 3 settlers are already (being) trained");
                SendSettlersTask(acc);
                return TaskRes.Executed;
            }

            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            var div = troopNode.Descendants("div");
            Vill.Troops.Settlers = (int)Parser.RemoveNonNumeric(div.FirstOrDefault(x => x.HasClass("tit")).Descendants("span").FirstOrDefault().InnerText);

            string innertext = "";
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    innertext = troopNode.ChildNodes.First(x => x.Name == "a").InnerText;
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    // no expansion slot
                    if (div.FirstOrDefault(x => x.HasClass("noExpansionSlot")) != null)
                    {
                        if (Vill.Troops.Settlers >= 3)
                        {
                            if (acc.NewVillages.AutoSettleNewVillages)
                            {
                                acc.Tasks.Add(new SendSettlers()
                                {
                                    ExecuteAt = DateTime.Now.AddHours(-3),
                                    Vill = this.Vill,
                                    // For high speed servers, you want to train settlers asap
                                    Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
                                }, true);
                            }

                            acc.Logger.Warning("Have enoung settlers");
                        }
                        else
                        {
                            acc.Logger.Warning("Don't have enough expansion slot or settlers are training.");
                        }
                        return TaskRes.Executed;
                    }

                    innertext = div.FirstOrDefault(x => x.HasClass("cta")).Descendants("a").FirstOrDefault().InnerText;
                    break;
            }
            var maxNum = Parser.RemoveNonNumeric(innertext);
            Vill.Troops.Settlers = (int)TroopsParser.ParseAvailable(troopNode);

            var costNode = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var cost = ResourceParser.GetResourceCost(costNode);

            if (!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
            {
                ResourcesHelper.NotEnoughRes(acc, Vill, cost, this);
                return TaskRes.Executed;
            }

            acc.Wb.ExecuteScript($"document.getElementsByName('t10')[0].value='{maxNum}'");
            await Task.Delay(AccountHelper.Delay());

            // Click Train button
            await TbsCore.Helpers.DriverHelper.ExecuteScript(acc, "document.getElementById('s1').click()");
            Vill.Troops.Settlers += (int)maxNum;

            if (Vill.Troops.Settlers < 3)
            {
                // random train next settlers after 30 - 60 mins
                var ran = new Random();
                this.NextExecute = DateTime.Now.AddMinutes(ran.Next(30, 60));
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

            acc.Logger.Information($"Bot will (try to) send settlers in {TimeHelper.InSeconds(training)} sec");

            acc.Tasks.Add(new SendSettlers()
            {
                ExecuteAt = training,
                Vill = this.Vill,
                // For high speed servers, you want to train settlers asap
                Priority = 1000 < acc.AccInfo.ServerSpeed ? TaskPriority.High : TaskPriority.Medium,
            }, true);
        }
    }
}