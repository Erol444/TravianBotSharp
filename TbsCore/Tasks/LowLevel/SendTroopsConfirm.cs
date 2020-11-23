using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendTroopsConfirm : BotTask
    {
        //TODO: add option for scouting type / catapult target(s)

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await Task.Delay(AccountHelper.Delay());
            wb.FindElementById("btn_ok").Click();

            return TaskRes.Executed;
        }
    }
}
