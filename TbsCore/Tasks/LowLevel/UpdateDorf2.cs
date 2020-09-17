using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateDorf2 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;
            //remove all same tasks
            TaskExecutor.RemoveSameTasksForVillage(acc, vill, this.GetType(), this);

            var url = $"{acc.AccInfo.ServerUrl}/dorf2.php";
            await acc.Wb.Navigate(url);
            return TaskRes.Executed;
        }
    }
}
