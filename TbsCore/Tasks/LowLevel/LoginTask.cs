using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class LoginTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            if (!TaskExecutor.IsLoginScreen(acc))
            {
                await Task.Delay(AccountHelper.Delay() * 2);
                return TaskRes.Executed;
            }

            var access = acc.Access.GetCurrentAccess();

            if (acc.AccInfo.ServerUrl.Contains("ttwars"))
            {
                wb.ExecuteScript($"document.getElementsByName('user')[0].value='{acc.AccInfo.Nickname}'");
                wb.ExecuteScript($"document.getElementsByName('pw')[0].value='{access.Password}'");
            }
            else
            {
                wb.ExecuteScript($"document.getElementsByName('name')[0].value='{acc.AccInfo.Nickname}'");
                wb.ExecuteScript($"document.getElementsByName('password')[0].value='{access.Password}'");
            }

            await DriverHelper.ExecuteScript(acc, "document.getElementsByName('s1')[0].click()");

            if (TaskExecutor.IsLoginScreen(acc))
            {
                // Wrong password/nickname
                acc.TaskTimer.Stop();

            }
            return TaskRes.Executed;
        }
    }
}
