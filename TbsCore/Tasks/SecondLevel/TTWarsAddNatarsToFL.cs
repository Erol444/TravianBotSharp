using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Helpers;
using TbsCore.Parsers;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Tasks.SecondLevel
{
    public class TTWarsAddNatarsToFL : BotTask
    {
        public int MinPop { get; set; }
        public int MaxPop { get; set; }
        public FarmList FL { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/spieler.php?uid=1");

            var vills = acc.Wb.Html.GetElementbyId("villages").Descendants("tr");
            int addedFarms = 0;
            foreach (var vill in vills)
            {
                var name = vill.ChildNodes.First(x => x.HasClass("name")).InnerText;
                var pop = (int)Parser.RemoveNonNumeric(vill.Descendants().First(x => x.HasClass("inhabitants")).InnerHtml);
                if (MinPop < pop && pop < MaxPop)
                {
                    var href = vill.Descendants("a").First(x => x.GetAttributeValue("href", "").StartsWith("karte.php?x=")).GetAttributeValue("href", "").Split('?')[1];
                    var xy = href.Split('&');

                    Coordinates coords = new Coordinates
                    {
                        x = (int)Parser.RemoveNonNumeric(xy[0].Split('=')[1]),
                        y = (int)Parser.RemoveNonNumeric(xy[1].Split('=')[1])
                    };

                    acc.Tasks.Add(new AddFarm()
                    {
                        ExecuteAt = DateTime.Now.AddMilliseconds(addedFarms),
                        Farm = new TbsCore.Models.VillageModels.Farm()
                        {
                            Troops = new int[] { 100 },
                            Coords = coords,
                        },
                        FarmListId = this.FL.Id,
                    });

                    addedFarms++;
                    if (FL.NumOfFarms + addedFarms >= 100) break; //no more slots FL slots!
                }
            }
            return TaskRes.Executed;
        }
    }
}