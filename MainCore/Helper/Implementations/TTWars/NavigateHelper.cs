using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace MainCore.Helper.Implementations.TTWars
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
                if (_checkHelper.IsWWPage(chromeBrowser)) return Result.Fail(new Stop("WW complete page found"));
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

            return Result.Ok();
        }
    }
}