using System.Threading.Tasks;
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
                await DriverHelper.WriteByName(acc, "user", acc.AccInfo.Nickname);
                await DriverHelper.WriteByName(acc, "pw", access.Password);
            }
            else
            {
                await DriverHelper.WriteByName(acc, "name", acc.AccInfo.Nickname);
                await DriverHelper.WriteByName(acc, "password", access.Password);
            }

            await DriverHelper.ClickByName(acc, "s1");

            if (TaskExecutor.IsLoginScreen(acc))
            {
                // Wrong password/nickname
                acc.Wb.Log("Password is incorrect!");
                acc.TaskTimer.Stop();
            }
            else
            {
                await TaskExecutor.PageLoaded(acc);
                // check sitter account
                var auction = acc.Wb.Html.DocumentNode.SelectSingleNode("//a[contains(@class,'auction')]");

                acc.Access.GetCurrentAccess().IsSittering = (auction != null && auction.HasClass("disable"));
            }

            return TaskRes.Executed;
        }
    }
}