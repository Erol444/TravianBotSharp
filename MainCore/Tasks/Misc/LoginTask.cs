using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using ModuleCore.Parser;
using OpenQA.Selenium;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class LoginTask : AccountBotTask
    {
        private readonly ICheckHelper _checkHelper;

        private readonly ISystemPageParser _systemPageParser;

        private readonly IPlanManager _planManager;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _checkHelper = Locator.Current.GetService<ICheckHelper>();
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
            _planManager = Locator.Current.GetService<IPlanManager>();
        }

        public override Result Execute()
        {
            {
                var result = AcceptCookie();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = Login();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
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
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
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
                return Result.Fail(new Retry("Cannot find username box"));
            }

            if (passwordNode is null)
            {
                return Result.Fail(new Retry("Cannot find password box"));
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();
            var chrome = _chromeBrowser.GetChrome();

            var usernameElement = chrome.FindElements(By.XPath(usernameNode.XPath));
            if (usernameElement.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find username box"));
            }
            var passwordElement = chrome.FindElements(By.XPath(passwordNode.XPath));
            if (passwordElement.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find password box"));
            }
            var buttonElements = chrome.FindElements(By.XPath(buttonNode.XPath));
            if (buttonElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find login button"));
            }

            usernameElement[0].SendKeys(Keys.Home);
            usernameElement[0].SendKeys(Keys.Shift + Keys.End);
            usernameElement[0].SendKeys(account.Username);

            passwordElement[0].SendKeys(Keys.Home);
            passwordElement[0].SendKeys(Keys.Shift + Keys.End);
            passwordElement[0].SendKeys(access.Password);

            buttonElements[0].Click();
            if (VersionDetector.IsTTWars())
            {
                html = _chromeBrowser.GetHtml();
                if (_checkHelper.IsSkipTutorial(html))
                {
                    var skipButton = html.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("questButtonSkipTutorial"));
                    if (skipButton is null)
                    {
                        return Result.Fail(new Retry("Cannot find skip quest button"));
                    }
                    var skipButtons = chrome.FindElements(By.XPath(skipButton.XPath));
                    if (skipButtons.Count == 0)
                    {
                        return Result.Fail(new Retry("Cannot find skip quest button"));
                    }
                    var result = _navigateHelper.Click(AccountId, skipButtons[0]);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            return Result.Ok();
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
                        _taskManager.Add(AccountId, new RefreshVillage(village.Id, AccountId));
                    }
                }
            }
        }
    }
}