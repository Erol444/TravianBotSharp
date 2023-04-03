using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parser.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class NavigateHelper : Base.NavigateHelper
    {
        public NavigateHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, INavigationBarParser navigationBarParser, IBuildingsHelper buildingsHelper, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IBuildingTabParser buildingTabParser, IHeroSectionParser heroSectionParser) : base(chromeManager, contextFactory, checkHelper, villagesTableParser, navigationBarParser, buildingsHelper, villageFieldParser, villageInfrastructureParser, buildingTabParser, heroSectionParser)
        {
        }

        public override Result AfterClicking(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            if (_checkHelper.IsCaptcha(html))
            {
                return Result.Fail(new Stop("Captcha found"));
            }
            if (_checkHelper.IsWWMsg(html))
            {
                return Result.Fail(new Stop("WW complete page found"));
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

        public override Result GoToBuilding(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

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

        public override Result ToHeroInventory(int accountId)
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

            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var tab = _heroSectionParser.GetHeroTab(doc, 1);
                if (tab is null) return false;
                return _heroSectionParser.IsCurrentTab(tab);
            });

            return Result.Ok();
        }

        public override Result ToAdventure(int accountId)
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

            return Result.Ok();
        }
    }
}