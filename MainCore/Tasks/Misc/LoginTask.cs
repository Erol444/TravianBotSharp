using MainCore.Helper;
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

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Tasks.Misc
{
    public class LoginTask : AccountBotTask
    {
        public LoginTask(int accountId) : base(accountId, "Login task")
        {
        }

        public override void Execute()
        {
            AcceptCookie();
            Login();
            AddTask();
        }

        private void AcceptCookie()
        {
            var html = _chromeBrowser.GetHtml();
            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var driver = _chromeBrowser.GetChrome();
                var acceptCookie = driver.FindElements(By.ClassName("cmpboxbtnyes"));
                acceptCookie[0].Click();
            }
        }

        private void Login()
        {
            var html = _chromeBrowser.GetHtml();

            var usernameNode = LoginPage.GetUsernameNode(html);

            var passwordNode = LoginPage.GetPasswordNode(html);

            var buttonNode = LoginPage.GetLoginButton(html);
            if (buttonNode is null)
            {
                _logManager.Information(AccountId, "Account is already logged in. Skip login task");
                return;
            }

            if (usernameNode is null)
            {
                throw new Exception("Cannot find username box");
            }

            if (passwordNode is null)
            {
                throw new Exception("Cannot find password box");
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = _chromeBrowser.GetChrome();

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

            var setting = context.AccountsSettings.Find(AccountId);
            NavigateHelper.Sleep(setting.ClickDelayMin, setting.ClickDelayMax);
            NavigateHelper.WaitPageChanged(_chromeBrowser, "dorf");
            NavigateHelper.WaitPageLoaded(_chromeBrowser);
            NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

#elif TTWARS
            html = _chromeBrowser.GetHtml();
            if (CheckHelper.IsSkipTutorial(html))
            {
                var skipButton = html.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("questButtonSkipTutorial"));
                if (skipButton is null)
                {
                    throw new Exception("Cannot find skip quest button");
                }
                var skipButtons = chrome.FindElements(By.XPath(skipButton.XPath));
                if (skipButtons.Count == 0)
                {
                    throw new Exception("Cannot find skip quest button");
                }
                skipButtons[0].Click();

                NavigateHelper.Sleep(setting.ClickDelayMin, setting.ClickDelayMax);
                NavigateHelper.WaitPageLoaded(_chromeBrowser);
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
#else

#error You forgot to define Travian version here

#endif
        }

        private void AddTask()
        {
            _taskManager.Add(AccountId, new UpdateInfo(AccountId));

            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = _taskManager.GetList(AccountId);
            var upgradeBuildingList = listTask.OfType<UpgradeBuilding>();
            var updateList = listTask.OfType<UpdateDorf1>();
            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    var upgradeBuilding = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (upgradeBuilding is null)
                    {
                        _taskManager.Add(AccountId, new UpgradeBuilding(village.Id, AccountId));
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var update = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        _taskManager.Add(AccountId, new UpdateDorf1(village.Id, AccountId));
                    }
                }
            }
        }
    }
}