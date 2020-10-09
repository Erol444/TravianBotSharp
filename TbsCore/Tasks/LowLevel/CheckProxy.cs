using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class CheckProxy : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Checking proxy " + acc.Access.GetCurrentAccess().Proxy);
            await acc.Wb.Navigate("https://api.ipify.org/");
            var ip = acc.Wb.Html.DocumentNode.InnerText;

            var currentProxy = acc.Access.GetCurrentAccess().Proxy;
            if (!string.IsNullOrEmpty(currentProxy) &&
                ip.Trim() != currentProxy.Trim())
            {
                // Proxy error!
                Utils.log.Information($"Failed proxy {currentProxy} for account {acc.AccInfo.Nickname}! Trying to get new proxy.");
                if (acc.Access.AllAccess.Count > 1)
                {
                    // Try another access.
                    var changeAccess = new ChangeAccess();
                    await changeAccess.Execute(acc);
                    return TaskRes.Executed;
                }
                else
                {
                    Utils.log.Information($"Will sleep and retry the same proxy..");
                    await Task.Delay(AccountHelper.Delay() * 15);
                    this.NextExecute = DateTime.MinValue.AddMinutes(1);
                }
            }
            else
            {
                // Proxy OK
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            }
            return TaskRes.Executed;
        }
    }
}
