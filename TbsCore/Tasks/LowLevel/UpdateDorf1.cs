using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateDorf1 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            //remove all same tasks
            TaskExecutor.RemoveSameTasksForVillage(acc, Vill, this.GetType(), this);

            var url = $"{acc.AccInfo.ServerUrl}/dorf1.php";
            await acc.Wb.Navigate(url);
            Vill.Timings.LastVillRefresh = DateTime.Now;
            return TaskRes.Executed;
        }
    }
}
