using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOffcialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficalNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.FindElements;

#endif

namespace MainCore.Tasks.Misc
{
    public class LoginTask : BotTask
    {
        public LoginTask(int accountId, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, ILogManager logManager, IDatabaseEvent databaseEvent)
            : base(accountId, contextFactory, chromeBrowser, taskManager, logManager, databaseEvent) { }

        public override Task Execute()
        {
            return Task.Run(Login);
        }

        private void Login()
        {
            var html = _chromeBrowser.GetHtml();

            var usernameNode = LoginPage.GetUsernameNode(html);
            if (usernameNode is null)
            {
                _logManager.Warning(_accountId, "[Login Task] Cannot find username box");
                _taskManager.UpdateAccountStatus(_accountId, AccountStatus.Offline);
                return;
            }
            var passwordNode = LoginPage.GetPasswordNode(html);
            if (passwordNode is null)
            {
                _logManager.Warning(_accountId, "[Login Task] Cannot find password box");
                _taskManager.UpdateAccountStatus(_accountId, AccountStatus.Offline);
                return;
            }
            var buttonNode = LoginPage.GetLoginButton(html);
            if (buttonNode is null)
            {
                _logManager.Warning(_accountId, "[Login Task] Cannot find login button");
                _taskManager.UpdateAccountStatus(_accountId, AccountStatus.Offline);
                return;
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(_accountId);
            var access = context.Accesses.Where(x => x.AccountId == _accountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = _chromeBrowser.GetChrome();

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

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains("dorf"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}