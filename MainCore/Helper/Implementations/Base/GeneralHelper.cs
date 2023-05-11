using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class GeneralHelper : IGeneralHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        protected readonly IChromeManager _chromeManager;

        protected readonly ICheckHelper _checkHelper;
        protected readonly IUpdateHelper _updateHelper;

        protected readonly INavigationBarParser _navigationBarParser;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly IBuildingTabParser _buildingTabParser;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public GeneralHelper(IChromeManager chromeManager, INavigationBarParser navigationBarParser, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, IDbContextFactory<AppDbContext> contextFactory, IBuildingTabParser buildingTabParser, IUpdateHelper updateHelper)
        {
            _chromeManager = chromeManager;
            _navigationBarParser = navigationBarParser;
            _checkHelper = checkHelper;
            _villagesTableParser = villagesTableParser;
            _contextFactory = contextFactory;
            _buildingTabParser = buildingTabParser;
            _updateHelper = updateHelper;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);

            _checkHelper.Load(villageId, accountId, cancellationToken);
            _updateHelper.Load(villageId, accountId, cancellationToken);
        }

        public bool IsPageValid()
        {
            var doc = _chromeBrowser.GetHtml();
            var resourceButton = _navigationBarParser.GetBuildingButton(doc);
            if (resourceButton is null) return false;
            return true;
        }

        public int GetDelayClick()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(_accountId);
            return Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax);
        }

        public void DelayClick() => Thread.Sleep(GetDelayClick());

        public Result WaitPageLoaded()
        {
            return Wait(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public Result WaitPageChanged(string path)
        {
            return Wait(driver => driver.Url.Contains(path));
        }

        public Result Wait(Func<IWebDriver, bool> condition)
        {
            var wait = _chromeBrowser.GetWait();
            try
            {
                wait.Until(condition);
            }
            catch (TimeoutException)
            {
                return Result.Fail(new Stop("Page not loaded in 3 mins"));
            }
            return Result.Ok();
        }

        public Result Click(By by, bool waitPageLoaded = true)
        {
            var chrome = _chromeBrowser.GetChrome();
            var elements = chrome.FindElements(by);

            if (elements.Count == 0) return Result.Fail(Retry.ElementNotFound);

            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Result.Fail(Retry.ElementCannotClick);
            element.Click();
            DelayClick();

            if (waitPageLoaded)
            {
                _result = WaitPageLoaded();
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result Click(By by, string path)
        {
            _result = Click(by, waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = WaitPageChanged(path);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = WaitPageLoaded();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result Input(By by, string content)
        {
            var chrome = _chromeBrowser.GetChrome();
            var elements = chrome.FindElements(by);

            if (elements.Count == 0) return Result.Fail(Retry.ElementNotFound);

            var element = elements[0];
            element.SendKeys(Keys.Home);
            element.SendKeys(Keys.Shift + Keys.End);
            element.SendKeys(content);

            return Result.Ok();
        }

        public Result Reload()
        {
            _chromeBrowser.Navigate();
            DelayClick();
            _result = WaitPageLoaded();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result Navigate(string url)
        {
            _chromeBrowser.Navigate(url);
            DelayClick();
            _result = WaitPageChanged(url);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            _result = WaitPageLoaded();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ToDorf1(bool forceReload = false)
        {
            const string dorf1 = "dorf1";
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains(dorf1))
            {
                if (forceReload)
                {
                    Reload();
                }
                return Result.Ok();
            }

            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var doc = _chromeBrowser.GetHtml();
            var node = _navigationBarParser.GetResourceButton(doc);
            if (node is null)
            {
                return Result.Fail(Retry.ButtonNotFound("resources"));
            }

            _result = Click(By.XPath(node.XPath), dorf1);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _updateHelper.UpdateDorf1();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ToDorf2(bool forceReload = false)
        {
            const string dorf2 = "dorf2";
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains(dorf2))
            {
                if (forceReload)
                {
                    Reload();
                }
                return Result.Ok();
            }

            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var doc = _chromeBrowser.GetHtml();
            var node = _navigationBarParser.GetBuildingButton(doc);
            if (node is null)
            {
                return Result.Fail(Retry.ButtonNotFound("buildings"));
            }

            _result = Click(By.XPath(node.XPath), dorf2);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _updateHelper.UpdateDorf2();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result<int> ToDorf(bool forceReload = false)
        {
            const string dorf = "dorf";
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains(dorf))
            {
                if (forceReload)
                {
                    return Reload();
                }
            }

            var chanceDorf2 = DateTime.Now.Ticks % 100;
            return chanceDorf2 >= 50 ? ToDorf2() : ToDorf1();
        }

        public Result SwitchVillage()
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            while (!_checkHelper.IsCorrectVillage())
            {
                if (_token.IsCancellationRequested) return Result.Fail(new Cancel());
                var html = _chromeBrowser.GetHtml();

                var listNode = _villagesTableParser.GetVillages(html);
                foreach (var node in listNode)
                {
                    var id = _villagesTableParser.GetId(node);
                    if (id != _villageId) continue;

                    var result = Click(By.XPath(node.XPath));
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    break;
                }
            }
            return Result.Ok();
        }

        public Result SwitchTab(int index)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);
            while (!_checkHelper.IsCorrectTab(index))
            {
                var html = _chromeBrowser.GetHtml();
                var listNode = _buildingTabParser.GetBuildingTabNodes(html);
                if (listNode.Count == 0)
                {
                    return Result.Fail(new Retry("Cannot find building tabs"));
                }
                if (index > listNode.Count)
                {
                    return Result.Fail(new Retry($"Tab {index} is invalid, this building only has {listNode.Count} tabs"));
                }

                _result = Click(By.XPath(listNode[index].XPath));
                if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
                return Result.Ok();
            }
            return Result.Ok();
        }

        public abstract Result ToBuilding(int index);

        public abstract Result ToHeroInventory();

        public abstract Result ToAdventure();

        public abstract Result ClickStartFarm(int farmId);
    }
}