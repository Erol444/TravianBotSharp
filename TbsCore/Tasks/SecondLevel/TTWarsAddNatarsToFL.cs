using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks.LowLevel;
using FarmList = TbsCore.Models.TroopsModels.FarmList;

namespace TravBotSharp.Files.Tasks.SecondLevel
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
            var addedFarms = 0;
            foreach (var vill in vills)
            {
                var name = vill.ChildNodes.First(x => x.HasClass("name")).InnerText;
                var pop = (int) Parser.RemoveNonNumeric(vill.Descendants().First(x => x.HasClass("inhabitants"))
                    .InnerHtml);
                if (MinPop < pop && pop < MaxPop)
                {
                    var href = vill.Descendants("a")
                        .First(x => x.GetAttributeValue("href", "").StartsWith("karte.php?x="))
                        .GetAttributeValue("href", "").Split('?')[1];
                    var xy = href.Split('&');

                    var coords = new Coordinates
                    {
                        x = (int) Parser.RemoveNonNumeric(xy[0].Split('=')[1]),
                        y = (int) Parser.RemoveNonNumeric(xy[1].Split('=')[1])
                    };

                    TaskExecutor.AddTask(acc, new AddFarm
                    {
                        ExecuteAt = DateTime.Now.AddMilliseconds(addedFarms),
                        Farm = new Farm
                        {
                            Troops = new[] {100},
                            Coords = coords
                        },
                        FarmListId = FL.Id
                    });

                    addedFarms++;
                    if (FL.NumOfFarms + addedFarms >= 100) break; //no more slots FL slots!
                }
            }

            return TaskRes.Executed;
        }
    }
}