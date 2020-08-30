using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class AcceptCookies : BotTask
    {
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            wb.ExecuteScript("document.getElementById('CybotCookiebotDialogBodyLevelButtonLevelOptinDeclineAll').click()");
            return TaskRes.Executed;
        }
    }
}