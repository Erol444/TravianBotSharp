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
        public LoginTask(int accountId, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IDatabaseEvent databaseEvent, ILogManager logManager)
            : base(accountId, contextFactory, chromeBrowser, taskManager, databaseEvent, logManager) { }

        public override async Task<TaskRes> Execute()
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(_accountId);

            var url = _chromeBrowser.GetCurrentUrl();
            if (!url.Contains(account.Server))
            {
                await Task.Run(() => _chromeBrowser.Navigate(account.Server));
            }

            _chromeBrowser.UpdateHtml();
            var html = _chromeBrowser.GetHtml();
            var usernameNode = LoginPage.GetUsernameNode(html);
            var passwordNode = LoginPage.GetPasswordNode(html);
            var buttonNode = LoginPage.GetLoginButton(html);

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

            var waiter = _chromeBrowser.GetWait();
            waiter.Until(driver => driver.Url.Contains("dorf"));
            waiter.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            return TaskRes.Executed;
        }
    }
}