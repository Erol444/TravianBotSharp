using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.SendTroopsModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendDeff : BotTask
    {
        public SendDeffAmount DeffAmount { get; set; }
        public SendDeff NextDeffTask { get; set; }
        public Coordinates TargetVillage { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");

            int[] troopsAtHome = TroopsMovementParser.GetTroopsInRallyPoint(acc.Wb.Html);

            for (int i = 0; i < 10; i++)
            {
                var troop = TroopsHelper.TroopFromInt(acc, i);
                if (TroopsHelper.IsTroopDefensive(troop))
                {
                    var upkeep = TroopSpeed.GetTroopUpkeep(troop);
                    int sendAmount = troopsAtHome[i];
                    if (this.DeffAmount.DeffCount != null)
                    {
                        if (sendAmount * upkeep > this.DeffAmount.DeffCount)
                        {
                            // We have sent all needed deff
                            sendAmount = (this.DeffAmount.DeffCount ?? 0) / upkeep;

                            // Remove all other (linked) SendDeff bot tasks
                            var list = new List<SendDeff>
                            {
                                this.NextDeffTask
                            };
                            while (list.Last() != null)
                            {
                                list.Add(list.Last().NextDeffTask);
                            }
                            foreach (var task in list)
                            {
                                if (task == null) continue;
                                acc.Tasks.Remove(task);
                            }
                        }
                        else
                        {
                            this.DeffAmount.DeffCount -= sendAmount * upkeep;
                        }
                    }

                    wb.ExecuteScript($"document.getElementsByName('t{i + 1 }')[0].value='{sendAmount}'");
                }
            }

            //select coordinates
            await wb.FindElementById("xCoordInput").Write(TargetVillage.x);
            await wb.FindElementById("yCoordInput").Write(TargetVillage.y);

            //Select reinforcement
            string script = "var radio = document.getElementsByClassName(\"radio\");for(var i = 0; i < radio.length; i++){";
            script += $"if(radio[i].value == '2') radio[i].checked = \"checked\"}}";
            await DriverHelper.ExecuteScript(acc, script);
            await acc.Wb.Driver.FindElementById("btn_ok").Click(acc);

            // Confirm
            wb.FindElementById("btn_ok").Click();
            return TaskRes.Executed;
        }
    }
}
