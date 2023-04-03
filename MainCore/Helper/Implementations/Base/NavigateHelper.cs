using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parser.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class NavigateHelper : INavigateHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly ICheckHelper _checkHelper;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly INavigationBarParser _navigationBarParser;
        protected readonly IBuildingsHelper _buildingsHelper;
        protected readonly IVillageFieldParser _villageFieldParser;
        protected readonly IVillageInfrastructureParser _villageInfrastructureParser;
        protected readonly IBuildingTabParser _buildingTabParser;
        protected readonly IHeroSectionParser _heroSectionParser;

        public NavigateHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, INavigationBarParser navigationBarParser, IBuildingsHelper buildingsHelper, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IBuildingTabParser buildingTabParser, IHeroSectionParser heroSectionParser)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _checkHelper = checkHelper;
            _villagesTableParser = villagesTableParser;
            _navigationBarParser = navigationBarParser;
            _buildingsHelper = buildingsHelper;
            _villageFieldParser = villageFieldParser;
            _villageInfrastructureParser = villageInfrastructureParser;
            _buildingTabParser = buildingTabParser;
            _heroSectionParser = heroSectionParser;
        }

        public Result WaitPageLoaded(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.GetWait();
            try
            {
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (TimeoutException)
            {
                return Result.Fail(new Stop("Page not loaded in 3 mins"));
            }
            return Result.Ok();
        }

        public Result WaitPageChanged(int accountId, string path)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.GetWait();
            try
            {
                wait.Until(driver => driver.Url.Contains(path));
            }
            catch
            {
            }
            return WaitPageLoaded(accountId);
        }

        public int GetDelayClick(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            return Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax);
        }

        public void Sleep(int min, int max) => Thread.Sleep(Random.Shared.Next(min, max));

        public abstract Result AfterClicking(int accountId);

        public Result SwitchVillage(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            while (!_checkHelper.IsCorrectVillage(accountId, villageId))
            {
                var html = chromeBrowser.GetHtml();

                var listNode = _villagesTableParser.GetVillages(html);
                foreach (var node in listNode)
                {
                    var id = _villagesTableParser.GetId(node);
                    if (id != villageId) continue;

                    var chrome = chromeBrowser.GetChrome();
                    var elements = chrome.FindElements(By.XPath(node.XPath));
                    var result = Click(accountId, elements[0]);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                    break;
                }
            }
            return Result.Ok();
        }

        public Result Click(int accountId, IWebElement element)
        {
            if (!element.Displayed || !element.Enabled) return Result.Fail(new Retry("Element is not clickable"));
            element.Click();
            var delay = GetDelayClick(accountId);
            Thread.Sleep(delay);

            {
                var result = WaitPageLoaded(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = AfterClicking(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        public Result ToDorf1(int accountId, bool isForce = false)

        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf1"))
            {
                if (isForce)
                {
                    chromeBrowser.Navigate();
                    var delay = GetDelayClick(accountId);
                    Thread.Sleep(delay);
                    {
                        var result = WaitPageChanged(accountId, "dorf1");
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var result = AfterClicking(accountId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }

                    if (!chromeBrowser.GetCurrentUrl().Contains("dorf1")) return ToDorf1(accountId, isForce);
                }

                return Result.Ok();
            }

            var doc = chromeBrowser.GetHtml();
            var node = _navigationBarParser.GetResourceButton(doc);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find Resources button"));
            }

            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find Resources button"));
            }

            return Click(accountId, elements[0]);
        }

        public Result ToDorf2(int accountId, bool isForce = false)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf2"))
            {
                if (isForce)
                {
                    chromeBrowser.Navigate();
                    var delay = GetDelayClick(accountId);
                    Thread.Sleep(delay);
                    {
                        var result = WaitPageChanged(accountId, "dorf2");
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var result = AfterClicking(accountId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }

                    if (!chromeBrowser.GetCurrentUrl().Contains("dorf2")) return ToDorf2(accountId, isForce);
                }

                return Result.Ok();
            }
            var doc = chromeBrowser.GetHtml();
            var node = _navigationBarParser.GetBuildingButton(doc);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find Buildings button"));
            }

            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find Buildings button"));
            }

            {
                var result = Click(accountId, elements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result GoRandomDorf(int accountId)
        {
            var chanceDorf2 = DateTime.Now.Ticks % 100;
            if (chanceDorf2 >= 50)
            {
                return ToDorf2(accountId);
            }
            else
            {
                return ToDorf1(accountId);
            }
        }

        public abstract Result GoToBuilding(int accountId, int index);

        public Result SwitchTab(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            while (!_checkHelper.IsCorrectTab(accountId, index))
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
                var chrome = chromeBrowser.GetChrome();
                var elements = chrome.FindElements(By.XPath(listNode[index].XPath));
                if (elements.Count == 0)
                {
                    return Result.Fail(new Retry("Cannot find building tabs"));
                }
                var result = Click(accountId, elements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                return Result.Ok();
            }
            return Result.Ok();
        }

        public abstract Result ToHeroInventory(int accountId);

        public abstract Result ToAdventure(int accountId);
    }
}