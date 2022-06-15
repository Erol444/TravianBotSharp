using HtmlAgilityPack;
using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Others
{
    public class LoginTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var usernameNode = GetUsernameNode(acc);
            if (usernameNode == null)
            {
                acc.Logger.Warning("Cannot find username box");
                acc.TaskTimer.Stop();
                acc.Status = Status.Paused;
                return TaskRes.Retry;
            }

            var passwordNode = GetPasswordNode(acc);
            if (passwordNode == null)
            {
                acc.Logger.Warning("Cannot find password box");
                acc.TaskTimer.Stop();
                acc.Status = Status.Paused;
                return TaskRes.Retry;
            }

            var buttonNode = GetLoginButton(acc);
            if (buttonNode == null)
            {
                acc.Logger.Warning("Cannot find login button");
                acc.TaskTimer.Stop();
                acc.Status = Status.Paused;
                return TaskRes.Retry;
            }

            var access = acc.Access.GetCurrentAccess();

            var usernameElement = acc.Wb.Driver.FindElement(By.XPath(usernameNode.XPath));

            usernameElement.SendKeys(Keys.Home);
            usernameElement.SendKeys(Keys.Shift + Keys.End);
            usernameElement.SendKeys(acc.AccInfo.Nickname);

            var passwordElement = acc.Wb.Driver.FindElement(By.XPath(passwordNode.XPath));
            passwordElement.SendKeys(Keys.Home);
            passwordElement.SendKeys(Keys.Shift + Keys.End);
            passwordElement.SendKeys(access.Password);

            var buttonElement = acc.Wb.Driver.FindElement(By.XPath(buttonNode.XPath));
            buttonElement.Click();

            var result = await DriverHelper.WaitPageChange(acc, "dorf");

            if (!result && TaskExecutor.IsLoginScreen(acc))
            {
                // Wrong password/nickname
                acc.Logger.Warning("Password is incorrect!");
                acc.TaskTimer.Stop();
                acc.Status = Status.Paused;
                return TaskRes.Retry;
            }
            else
            {
                // check sitter account
                var auction = acc.Wb.Html.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("auction"));
                acc.Access.GetCurrentAccess().IsSittering = (auction != null && auction.HasClass("disable"));
                return TaskRes.Executed;
            }
        }

        private HtmlNode GetUsernameNode(Account acc)
        {
            return acc.Wb.Html.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("name"));
        }

        private HtmlNode GetPasswordNode(Account acc)
        {
            return acc.Wb.Html.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("password"));
        }

        private HtmlNode GetLoginButton(Account acc)
        {
            var trNode = acc.Wb.Html.DocumentNode.Descendants("tr").FirstOrDefault(x => x.HasClass("loginButtonRow"));
            if (trNode == null) return null;
            return trNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        }
    }
}