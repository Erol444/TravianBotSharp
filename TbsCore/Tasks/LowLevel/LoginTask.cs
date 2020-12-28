using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class LoginTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            if (!TaskExecutor.IsLoginScreen(acc))
            {
                await Task.Delay(AccountHelper.Delay() * 2);
                return TaskRes.Executed;
            }

            var access = acc.Access.GetCurrentAccess();

            if (acc.AccInfo.ServerUrl.Contains("ttwars"))
            {
                acc.Wb.Driver.ExecuteScript($"document.getElementsByName('user')[0].value='{acc.AccInfo.Nickname}'");
                acc.Wb.Driver.ExecuteScript($"document.getElementsByName('pw')[0].value='{access.Password}'");
            }
            else
            {
                acc.Wb.Driver.ExecuteScript($"document.getElementsByName('name')[0].value='{acc.AccInfo.Nickname}'");
                acc.Wb.Driver.ExecuteScript($"document.getElementsByName('password')[0].value='{access.Password}'");
            }

            await DriverHelper.ExecuteScript(acc, "document.getElementsByName('s1')[0].click()");

            if (TaskExecutor.IsLoginScreen(acc))
            {
                // Wrong password/nickname
                acc.Wb.Log("Password is incorrect!");
                acc.TaskTimer.Stop();

            }
            return TaskRes.Executed;
        }
    }
}
