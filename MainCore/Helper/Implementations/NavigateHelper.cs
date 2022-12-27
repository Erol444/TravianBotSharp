using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ModuleCore.Parser;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations
{
    public class NavigateHelper : INavigateHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ICheckHelper _checkHelper;
        private readonly IVillagesTableParser _villagesTableParser;
        private readonly INavigationBarParser _navigationBarParser;
        private readonly IBuildingsHelper _buildingsHelper;
        private readonly IVillageFieldParser _villageFieldParser;
        private readonly IVillageInfrastructureParser _villageInfrastructureParser;
        private readonly IBuildingTabParser _buildingTabParser;
        private readonly IHeroSectionParser _heroSectionParser;

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

        public Result AfterClicking(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            if (_checkHelper.IsCaptcha(html))
            {
                return Result.Fail(new Stop("Captcha found"));
            }
            if (_checkHelper.IsWWMsg(html))
            {
                if (VersionDetector.IsTravianOfficial()) return Result.Fail(new Stop("WW complete page found"));
                if (VersionDetector.IsTTWars() && _checkHelper.IsWWPage(chromeBrowser)) return Result.Fail(new Stop("WW complete page found"));
            }

            if (_checkHelper.IsBanMsg(html))
            {
                return Result.Fail(new Stop("Ban page found"));
            }

            if (_checkHelper.IsMaintanance(html))
            {
                return Result.Fail(new Stop("Maintanance page found"));
            }

            if (_checkHelper.IsLoginScreen(html))
            {
                return Result.Fail(new Login());
            }
            if (_checkHelper.IsSysMsg(html))
            {
                var url = chromeBrowser.GetCurrentUrl();
                var serverUrl = new Uri(url);
                chromeBrowser.Navigate($"{serverUrl.Scheme}://{serverUrl.Host}/dorf1.php?ok=1");
                var delay = GetDelayClick(accountId);
                Thread.Sleep(delay);
                {
                    var result = WaitPageChanged(accountId, "dorf1.php");
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                {
                    var result = WaitPageLoaded(accountId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            return Result.Ok();
        }

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

        public Result GoToBuilding(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            if (VersionDetector.IsTravianOfficial())
            {
                var dorf = _buildingsHelper.GetDorf(index);
                var chrome = chromeBrowser.GetChrome();
                var html = chromeBrowser.GetHtml();
                switch (dorf)
                {
                    case 1:
                        {
                            var node = _villageFieldParser.GetNode(html, index);
                            if (node is null)
                            {
                                return Result.Fail(new Retry($"Cannot find resource field at {index}"));
                            }
                            var elements = chrome.FindElements(By.XPath(node.XPath));
                            if (elements.Count == 0)
                            {
                                return Result.Fail(new Retry($"Cannot find resource field at {index}"));
                            }
                            var result = Click(accountId, elements[0]);
                            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                        }
                        break;

                    case 2:
                        {
                            var node = _villageInfrastructureParser.GetNode(html, index);
                            if (node is null)
                            {
                                return Result.Fail(new Retry($"Cannot find building field at {index}"));
                            }
                            var pathBuilding = node.Descendants("path").FirstOrDefault();
                            if (pathBuilding is null)
                            {
                                return Result.Fail(new Retry($"Cannot find building field at {index}"));
                            }
                            var href = pathBuilding.GetAttributeValue("onclick", "");
                            var script = href.Replace("&amp;", "&");
                            chrome.ExecuteScript(script);
                            {
                                var result = WaitPageChanged(accountId, $"?id={index}");
                                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else if (VersionDetector.IsTTWars())
            {
                var currentUrl = chromeBrowser.GetCurrentUrl();
                var uri = new Uri(currentUrl);
                var serverUrl = $"{uri.Scheme}://{uri.Host}";
                var url = $"{serverUrl}/build.php?id={index}";
                chromeBrowser.Navigate(url);
                {
                    var result = WaitPageChanged(accountId, $"?id={index}");
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }

            {
                var result = WaitPageLoaded(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = AfterClicking(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            if (!chromeBrowser.GetCurrentUrl().Contains($"?id={index}")) return GoToBuilding(accountId, index);
            return Result.Ok();
        }

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

        public Result ToHeroInventory(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var avatar = _heroSectionParser.GetHeroAvatar(html);
            if (avatar is null)
            {
                return Result.Fail(new Retry("Cannot find hero avatar"));
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(avatar.XPath));
            if (elements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find hero avatar"));
            }

            var result = Click(accountId, elements[0]);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            if (VersionDetector.IsTravianOfficial())
            {
                var wait = chromeBrowser.GetWait();
                wait.Until(driver =>
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);
                    var tab = _heroSectionParser.GetHeroTab(doc, 1);
                    if (tab is null) return false;
                    return _heroSectionParser.IsCurrentTab(tab);
                });
            }

            return Result.Ok();
        }

        public Result ToAdventure(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetAdventuresButton(html);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));

            if (elements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }

            var result = Click(accountId, elements[0]);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            if (VersionDetector.IsTravianOfficial())
            {
                var wait = chromeBrowser.GetWait();
                wait.Until(driver =>
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);
                    var adventureDiv = doc.GetElementbyId("heroAdventure");
                    if (adventureDiv is null) return false;
                    var heroState = adventureDiv.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
                    if (heroState is null) return false;
                    return driver.FindElements(By.XPath(heroState.XPath)).Count > 0;
                });
            }
            return Result.Ok();
        }
    }
}