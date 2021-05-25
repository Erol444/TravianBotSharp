using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// TODO: replace selenium navigation with RestSharp client!
    /// ProxyHelper.TestProxy(acc);
    /// </summary>
    public class CheckProxy : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var currentProxy = acc.Access.GetCurrentAccess().Proxy;
            acc.Logger.Information("Checking proxy " + currentProxy);

            await acc.Wb.Navigate("https://api.ipify.org/");
            var ip = acc.Wb.Html.DocumentNode.InnerText;

            if (!string.IsNullOrEmpty(currentProxy) &&
                ip.Trim() != currentProxy.Trim())
            {
                // Proxy error!
                acc.Logger.Warning($"Proxy {currentProxy} doesn't work! Trying different proxy");
                if (acc.Access.AllAccess.Count > 1)
                {
                    // Try another access.
                    var changeAccess = new ChangeAccess();
                    await changeAccess.Execute(acc);
                    return TaskRes.Executed;
                }
                else
                {
                    acc.Logger.Warning($"There's only one access to this account! Will retry same proxy after 1 min...");
                    await Task.Delay(AccountHelper.Delay() * 15);
                    this.NextExecute = DateTime.MinValue.AddMinutes(1);
                }
            }
            else
            {
                // Proxy OK
                acc.Logger.Information($"Proxy OK!");
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/login.php");
            }
            return TaskRes.Executed;
        }
    }
}