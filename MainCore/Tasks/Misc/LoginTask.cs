using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using ServerModuleCore.Parser;
using Splat;
using System.Linq;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace MainCore.Tasks.Misc
{
    public class LoginTask : AccountBotTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly INavigateHelper _navigateHelper;
        private readonly ISystemPageParser _systemPageParser;
        private readonly ILogManager _logManager;
        private readonly ICheckHelper _checkHelper;
        private readonly ITaskManager _taskManager;
        private readonly IPlanManager _planManager;
        private IChromeBrowser _chromeBrowser;

        public LoginTask(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser, ICheckHelper checkHelper, ILogManager logManager, INavigateHelper navigateHelper, ITaskManager taskManager)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
            _checkHelper = checkHelper;
            _logManager = logManager;
            _navigateHelper = navigateHelper;
            _taskManager = taskManager;

            Name = "Login";
        }

        public override Result Execute()
        {
            _chromeBrowser = _chromeManager.Get(AccountId);
            {
                var result = AcceptCookie();
                if (result.IsFailed) return result.WithError("from login task");
            }
            {
                var result = Login();
                if (result.IsFailed) return result.WithError("from login task");
            }
            AddTask();
            return Result.Ok();
        }

        private Result AcceptCookie()
        {
            var html = _chromeBrowser.GetHtml();
            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var driver = _chromeBrowser.GetChrome();
                var acceptCookie = driver.FindElements(By.ClassName("cmpboxbtnyes"));
                var result = _navigateHelper.Click(AccountId, acceptCookie[0]);
                if (result.IsFailed) return result.WithError("from accept cookie");
            }
            return Result.Ok();
        }

        private Result Login()
        {
            var html = _chromeBrowser.GetHtml();

            var usernameNode = _systemPageParser.GetUsernameNode(html);

            var passwordNode = _systemPageParser.GetPasswordNode(html);

            var buttonNode = _systemPageParser.GetLoginButton(html);
            if (buttonNode is null)
            {
                _logManager.Information(AccountId, "Account is already logged in. Skip login task");
                return Result.Ok();
            }

            if (usernameNode is null)
            {
                return Result.Fail(new MustRetry("Cannot find username box"));
            }

            if (passwordNode is null)
            {
                return Result.Fail(new MustRetry("Cannot find password box"));
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = _chromeBrowser.GetChrome();

            var usernameElement = chrome.FindElements(By.XPath(usernameNode.XPath));
            if (usernameElement.Count == 0)
            {
                return Result.Fail(new MustRetry("Cannot find username box"));
            }
            var passwordElement = chrome.FindElements(By.XPath(passwordNode.XPath));
            if (passwordElement.Count == 0)
            {
                return Result.Fail(new MustRetry("Cannot find password box"));
            }
            var buttonElements = chrome.FindElements(By.XPath(buttonNode.XPath));
            if (buttonElements.Count == 0)
            {
                return Result.Fail(new MustRetry("Cannot find login button"));
            }

            usernameElement[0].SendKeys(Keys.Home);
            usernameElement[0].SendKeys(Keys.Shift + Keys.End);
            usernameElement[0].SendKeys(account.Username);

            passwordElement[0].SendKeys(Keys.Home);
            passwordElement[0].SendKeys(Keys.Shift + Keys.End);
            passwordElement[0].SendKeys(access.Password);

            {
                var result = _navigateHelper.Click(AccountId, buttonElements[0]);
                if (result.IsFailed) return result;
            }

            if (VersionDetector.IsTTWars())
            {
                html = _chromeBrowser.GetHtml();
                if (_checkHelper.IsSkipTutorial(html))
                {
                    var skipButton = html.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("questButtonSkipTutorial"));
                    var skipButtons = chrome.FindElements(By.XPath(skipButton.XPath));
                    var result = _navigateHelper.Click(AccountId, skipButtons[0]);
                    if (result.IsFailed) return result.WithError("from skip tutorial");
                }
            }
            return Result.Ok();
        }

        private void AddTask()
        {
            var updateInfoTask = Locator.Current.GetService<UpdateInfo>();
            updateInfoTask.SetAccountId(AccountId);
            _taskManager.Add(AccountId, updateInfoTask);

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
                        var upgradeBuildingTask = Locator.Current.GetService<UpgradeBuilding>();
                        upgradeBuildingTask.SetAccountId(AccountId);
                        upgradeBuildingTask.SetVillageId(village.Id);
                        _taskManager.Add(AccountId, upgradeBuildingTask);
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var update = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        var refreshVillageTask = Locator.Current.GetService<RefreshVillage>();
                        refreshVillageTask.SetAccountId(AccountId);
                        refreshVillageTask.SetVillageId(village.Id);
                        _taskManager.Add(AccountId, refreshVillageTask);
                    }
                }
            }
        }
    }
}