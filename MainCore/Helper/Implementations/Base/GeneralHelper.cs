using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class GeneralHelper : IGeneralHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        protected readonly IChromeManager _chromeManager;

        protected readonly ICheckHelper _checkHelper;
        protected readonly IUpdateHelper _updateHelper;
        protected readonly IInvalidPageHelper _invalidPageHelper;

        protected readonly INavigationBarParser _navigationBarParser;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly IBuildingTabParser _buildingTabParser;

        protected readonly IBuildingsHelper _buildingsHelper;
        protected readonly IVillageFieldParser _villageFieldParser;
        protected readonly IVillageInfrastructureParser _villageInfrastructureParser;
        protected readonly IHeroSectionParser _heroSectionParser;

        public GeneralHelper(IChromeManager chromeManager, INavigationBarParser navigationBarParser, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, IDbContextFactory<AppDbContext> contextFactory, IBuildingTabParser buildingTabParser, IUpdateHelper updateHelper, IInvalidPageHelper invalidPageHelper, IBuildingsHelper buildingsHelper, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IHeroSectionParser heroSectionParser)
        {
            _chromeManager = chromeManager;
            _navigationBarParser = navigationBarParser;
            _checkHelper = checkHelper;
            _villagesTableParser = villagesTableParser;
            _contextFactory = contextFactory;
            _buildingTabParser = buildingTabParser;
            _updateHelper = updateHelper;
            _invalidPageHelper = invalidPageHelper;
            _buildingsHelper = buildingsHelper;
            _villageFieldParser = villageFieldParser;
            _villageInfrastructureParser = villageInfrastructureParser;
            _heroSectionParser = heroSectionParser;
        }

        public abstract Result ToBuilding(int accountId, int villageId, int index);

        public abstract Result ToHeroInventory(int accountId);

        public int GetDelayClick(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            return Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax);
        }

        public void DelayClick(int accountId)
        {
            Thread.Sleep(GetDelayClick(accountId));
        }

        public Result Wait(int accountId, Func<IWebDriver, bool> condition)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.GetWait();
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

        public Result WaitPageLoaded(int accountId)
        {
            var result = Wait(accountId, driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result WaitPageChanged(int accountId, string path)
        {
            var result = Wait(accountId, driver => driver.Url.Contains(path));
            if (result.IsFailed) return result.WithError($"Browser cannot change to path [{path}]").WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result Click(int accountId, By by, bool waitPageLoaded = true)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(by);

            if (elements.Count == 0) return Result.Fail(Retry.ElementNotFound);

            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Result.Fail(Retry.ElementCannotClick);
            element.Click();
            DelayClick(accountId);

            if (waitPageLoaded)
            {
                var result = WaitPageLoaded(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result Click(int accountId, By by, string path)
        {
            var result = Click(accountId, by, waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = WaitPageChanged(accountId, path);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = WaitPageLoaded(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _invalidPageHelper.CheckPage(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result Input(int accountId, By by, string content)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(by);

            if (elements.Count == 0) return Result.Fail(Retry.ElementNotFound);

            var element = elements[0];
            element.SendKeys(Keys.Home);
            element.SendKeys(Keys.Shift + Keys.End);
            element.SendKeys(content);

            return Result.Ok();
        }

        public Result Reload(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            chromeBrowser.Navigate();
            DelayClick(accountId);
            var result = WaitPageLoaded(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _invalidPageHelper.CheckPage(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result Navigate(int accountId, string url)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            chromeBrowser.Navigate(url);
            DelayClick(accountId);
            var result = WaitPageChanged(accountId, url);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = WaitPageLoaded(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = _invalidPageHelper.CheckPage(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ToDorf1(int accountId, int villageId, bool forceReload = false, bool switchVillage = false)
        {
            Result result;
            if (switchVillage)
            {
                result = SwitchVillage(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            const string dorf1 = "dorf1";
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains(dorf1))
            {
                if (forceReload)
                {
                    result = Reload(accountId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }

                result = _updateHelper.UpdateDorf1(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                return Result.Ok();
            }

            var doc = chromeBrowser.GetHtml();
            var node = _navigationBarParser.GetResourceButton(doc);
            if (node is null)
            {
                return Result.Fail(Retry.ButtonNotFound("resources"));
            }

            result = Click(accountId, By.XPath(node.XPath), dorf1);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _updateHelper.UpdateDorf1(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ToDorf2(int accountId, int villageId, bool forceReload = false, bool switchVillage = false)
        {
            Result result;
            if (switchVillage)
            {
                result = SwitchVillage(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            const string dorf2 = "dorf2";
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains(dorf2))
            {
                if (forceReload)
                {
                    result = Reload(accountId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }

                result = _updateHelper.UpdateDorf2(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                return Result.Ok();
            }

            var doc = chromeBrowser.GetHtml();
            var node = _navigationBarParser.GetBuildingButton(doc);
            if (node is null)
            {
                return Result.Fail(Retry.ButtonNotFound("buildings"));
            }

            result = Click(accountId, By.XPath(node.XPath), dorf2);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _updateHelper.UpdateDorf2(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ToDorf(int accountId, int villageId, bool forceReload = false)
        {
            var result = SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            const string dorf = "dorf";
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains(dorf))
            {
                if (forceReload)
                {
                    result = Reload(accountId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }

                if (currentUrl.Contains("dorf1"))
                {
                    result = _updateHelper.UpdateDorf1(accountId, villageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                else if (currentUrl.Contains("dorf2"))
                {
                    result = _updateHelper.UpdateDorf2(accountId, villageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                return Result.Ok();
            }

            var chanceDorf2 = DateTime.Now.Ticks % 100;
            return chanceDorf2 >= 50 ? ToDorf2(accountId, villageId) : ToDorf1(accountId, villageId);
        }

        public Result ToBothDorf(int accountId, int villageId)
        {
            var result = SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var commands = new List<Func<Result>>();
            var chromeBrowser = _chromeManager.Get(accountId);
            var url = chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                commands.Add(() => ToDorf2(accountId, villageId));
                commands.Add(() => ToDorf1(accountId, villageId));
            }
            else if (url.Contains("dorf1"))
            {
                commands.Add(() => ToDorf1(accountId, villageId));
                commands.Add(() => ToDorf2(accountId, villageId));
            }
            else
            {
                if (Random.Shared.Next(0, 100) > 50)
                {
                    commands.Add(() => ToDorf1(accountId, villageId));
                    commands.Add(() => ToDorf2(accountId, villageId));
                }
                else
                {
                    commands.Add(() => ToDorf2(accountId, villageId));
                    commands.Add(() => ToDorf1(accountId, villageId));
                }
            }

            foreach (var command in commands)
            {
                result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result SwitchVillage(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            while (true)
            {
                var resultIsCorrectVillage = _checkHelper.IsCorrectVillage(accountId, villageId);
                if (resultIsCorrectVillage.IsFailed) return Result.Fail(resultIsCorrectVillage.Errors).WithError(new Trace(Trace.TraceMessage()));

                if (resultIsCorrectVillage.Value) return Result.Ok();

                var html = chromeBrowser.GetHtml();

                var listNode = _villagesTableParser.GetVillages(html);
                foreach (var node in listNode)
                {
                    var id = _villagesTableParser.GetId(node);
                    if (id != villageId) continue;

                    var result = Click(accountId, By.XPath(node.XPath));
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    break;
                }
            }
        }

        public Result SwitchTab(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            while (_checkHelper.IsCorrectTab(accountId, index))
            {
                var html = chromeBrowser.GetHtml();
                var listNode = _buildingTabParser.GetBuildingTabNodes(html);
                if (listNode.Count == 0)
                {
                    return Result.Fail(new Retry("Cannot find building tabs"));
                }
                if (index > listNode.Count)
                {
                    return Result.Fail(new Retry($"Tab {index} is invalid, this building only has {listNode.Count} tabs"));
                }

                var result = Click(accountId, By.XPath(listNode[index].XPath));
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                return Result.Ok();
            }
            return Result.Ok();
        }
    }
}