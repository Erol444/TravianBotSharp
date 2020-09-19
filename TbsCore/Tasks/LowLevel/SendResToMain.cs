using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{

    /// <summary>
    /// Send all resources (except 30k crop(TODO: SELECTABLE)) above 20% (todo: selectable) to main village.
    /// If we have auto celebration selected, leave res for that (calculate based on production)
    /// </summary>
    public class SendResToMain : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            var building = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Classificator.BuildingEnum.Marketplace);
            if (building == null)
            {
                //update dorg, no buildingId found?
                TaskExecutor.AddTask(acc, new UpdateDorf2() { ExecuteAt = DateTime.Now, Vill = Vill });
                Console.WriteLine($"There is no {building} in this village!");
                return TaskRes.Executed;
            }
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}&t=5");

            if (this.Vill.Settings.Type == Models.Settings.VillType.Support && this.Vill.Settings.SendRes)
            {
                // Repeat this task
                this.NextExecute = DateTime.Now.AddHours(1);
            }

            var mainVill = AccountHelper.GetMainVillage(acc);

            var res = MarketHelper.GetResToMainVillage(Vill);

            var ret = await MarketHelper.MarketSendResource(acc, res, mainVill, this);
            return TaskRes.Executed;
        }
    }
}
