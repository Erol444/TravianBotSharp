using MainCore.Enums;
using MainCore.Services;
using MainCore.Tasks.Update;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.FindElements;

#endif

namespace MainCore.Tasks.Misc
{
    public class LoginTask : BotTask
    {
        public LoginTask(int accountId) : base(accountId)
        {
        }

        public override Task Execute()
        {
            return Task.Run(() =>
            {
                AcceptCookie();
                var result = Login();
                if (!result) return;
                TaskManager.Add(AccountId, new UpdateInfo(AccountId));
            });
        }

        public override string Name => "Login Task";

        private void AcceptCookie()
        {
            var html = ChromeBrowser.GetHtml();
            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var driver = ChromeBrowser.GetChrome();
                var acceptCookie = driver.FindElements(By.ClassName("cmpboxbtnyes"));
                acceptCookie[0].Click();
            }
        }

        private bool Login()
        {
            var html = ChromeBrowser.GetHtml();

            var usernameNode = LoginPage.GetUsernameNode(html);
            if (usernameNode is null)
            {
                LogManager.Warning(AccountId, "[Login Task] Cannot find username box");
                TaskManager.UpdateAccountStatus(AccountId, AccountStatus.Offline);
                return false;
            }
            var passwordNode = LoginPage.GetPasswordNode(html);
            if (passwordNode is null)
            {
                LogManager.Warning(AccountId, "[Login Task] Cannot find password box");
                TaskManager.UpdateAccountStatus(AccountId, AccountStatus.Offline);
                return false;
            }
            var buttonNode = LoginPage.GetLoginButton(html);
            if (buttonNode is null)
            {
                LogManager.Warning(AccountId, "[Login Task] Cannot find login button");
                TaskManager.UpdateAccountStatus(AccountId, AccountStatus.Offline);
                return false;
            }

            using var context = ContextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = ChromeBrowser.GetChrome();

            var usernameElement = chrome.FindElements(By.XPath(usernameNode.XPath));
            usernameElement[0].SendKeys(Keys.Home);
            usernameElement[0].SendKeys(Keys.Shift + Keys.End);
            usernameElement[0].SendKeys(account.Username);

            var passwordElement = chrome.FindElements(By.XPath(passwordNode.XPath));
            passwordElement[0].SendKeys(Keys.Home);
            passwordElement[0].SendKeys(Keys.Shift + Keys.End);
            passwordElement[0].SendKeys(access.Password);

            var buttonElement = chrome.FindElements(By.XPath(buttonNode.XPath));
            buttonElement[0].Click();

            var wait = ChromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains("dorf"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            return true;
        }
    }
}