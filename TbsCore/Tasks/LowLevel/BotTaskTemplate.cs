using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class BotTaskTemplate : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            return TaskRes.Executed;
        }
    }
}
