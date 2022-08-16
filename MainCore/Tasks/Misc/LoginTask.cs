using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using OpenQA.Selenium;
using System;
using System.Linq;

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

        public override void Execute()
        {
            AcceptCookie();
            Login();
            AddTask();
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

        private void Login()
        {
            var html = ChromeBrowser.GetHtml();

            var usernameNode = LoginPage.GetUsernameNode(html);
            if (usernameNode is null)
            {
                throw new Exception("Cannot find username box");
            }
            var passwordNode = LoginPage.GetPasswordNode(html);
            if (passwordNode is null)
            {
                throw new Exception("Cannot find password box");
            }
            var buttonNode = LoginPage.GetLoginButton(html);
            if (buttonNode is null)
            {
                throw new Exception("Cannot find login button");
            }

            using var context = ContextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = ChromeBrowser.GetChrome();

            var usernameElement = chrome.FindElements(By.XPath(usernameNode.XPath));
            if (usernameElement.Count == 0)
            {
                throw new Exception("Cannot find username box");
            }
            var passwordElement = chrome.FindElements(By.XPath(passwordNode.XPath));
            if (passwordElement.Count == 0)
            {
                throw new Exception("Cannot find password box");
            }
            var buttonElements = chrome.FindElements(By.XPath(buttonNode.XPath));
            if (buttonElements.Count == 0)
            {
                throw new Exception("Cannot find login button");
            }

            usernameElement[0].SendKeys(Keys.Home);
            usernameElement[0].SendKeys(Keys.Shift + Keys.End);
            usernameElement[0].SendKeys(account.Username);

            passwordElement[0].SendKeys(Keys.Home);
            passwordElement[0].SendKeys(Keys.Shift + Keys.End);
            passwordElement[0].SendKeys(access.Password);

            buttonElements[0].Click();

            var wait = ChromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains("dorf"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private void AddTask()
        {
            TaskManager.Add(AccountId, new UpdateInfo(AccountId));

            using var context = ContextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = TaskManager.GetList(AccountId);
            var upgradeBuildingList = listTask.Where(x => x.GetType() == typeof(UpgradeBuilding)).OfType<UpgradeBuilding>();
            foreach (var village in villages)
            {
                var queue = context.VillagesQueueBuildings.Where(x => x.VillageId == village.Id);
                if (queue.Any())
                {
                    var upgradeBuilding = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (upgradeBuilding is null)
                    {
                        TaskManager.Add(AccountId, new UpgradeBuilding(village.Id, AccountId));
                    }
                }
            }
        }
    }
}