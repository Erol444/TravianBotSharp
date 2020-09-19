using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.TroopsModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks.LowLevel;

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
            int addedFarms = 0;
            foreach (var vill in vills)
            {
                var name = vill.ChildNodes.First(x => x.HasClass("name")).InnerText;
                var pop = (int)Parser.RemoveNonNumeric(vill.Descendants().First(x => x.HasClass("inhabitants")).InnerHtml);
                if (pop > MinPop && pop < MaxPop)
                {
                    var href = vill.Descendants("a").First(x => x.GetAttributeValue("href", "").StartsWith("karte.php?x=")).GetAttributeValue("href", "").Split('?')[1];
                    var xy = href.Split('&');
                    Coordinates coords = new Coordinates
                    {
                        x = (int)Parser.RemoveNonNumeric(xy[0].Split('=')[1]),
                        y = (int)Parser.RemoveNonNumeric(xy[1].Split('=')[1])
                    };
                    var task = new AddFarm()
                    {
                        Coordinates = coords,
                        ExecuteAt = DateTime.Now.AddMilliseconds(addedFarms),
                        FarmListId = this.FL.Id,
                        Troops = new List<TroopsRaw>() { new TroopsRaw() { Type = 1, Number = 10 } } //just add 10 of 1st troops
                    };
                    TaskExecutor.AddTask(acc, task);
                    addedFarms++;
                    if (FL.NumOfFarms + addedFarms >= 100) break; //no more slots FL slots!
                }
            }
            return TaskRes.Executed;
        }
    }
}
