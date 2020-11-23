using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TbsCore.Extensions;
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
                await acc.Wb.Driver.FindElementByName("user").Write(acc.AccInfo.Nickname);
                await acc.Wb.Driver.FindElementByName("pw").Write(access.Password);
            }
            else
            {
                await acc.Wb.Driver.FindElementByName("name").Write(acc.AccInfo.Nickname);
                await acc.Wb.Driver.FindElementByName("password").Write(access.Password);
            }

            await acc.Wb.Driver.FindElementByName("s1").Click(acc);

            if (TaskExecutor.IsLoginScreen(acc))
            {
                // Wrong password/nickname
                acc.TaskTimer.Stop();

            }
            return TaskRes.Executed;
        }
    }
}
