using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace MainCore.Helper.Implementations.TTWars
{
    public class GeneralHelper : Base.GeneralHelper
    {
        private readonly IHeroSectionParser _heroSectionParser;

        public GeneralHelper(IChromeManager chromeManager, INavigationBarParser navigationBarParser, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, IDbContextFactory<AppDbContext> contextFactory, IBuildingTabParser buildingTabParser, IHeroSectionParser heroSectionParser) : base(chromeManager, navigationBarParser, checkHelper, villagesTableParser, contextFactory, buildingTabParser)
        {
            _heroSectionParser = heroSectionParser;
        }

        public override Result ToAdventure()
        {
            var html = _chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetAdventuresButton(html);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }

            _result = Click(By.XPath(node.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public override Result ToBuilding(int index)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var currentUrl = _chromeBrowser.GetCurrentUrl();
            var uri = new Uri(currentUrl);
            var serverUrl = $"{uri.Scheme}://{uri.Host}";
            var url = $"{serverUrl}/build.php?id={index}";
            _result = Navigate(url);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public override Result ToHeroInventory()
        {
            var html = _chromeBrowser.GetHtml();
            var avatar = _heroSectionParser.GetHeroAvatar(html);
            if (avatar is null)
            {
                return Result.Fail(new Retry("Cannot find hero avatar"));
            }

            _result = Click(By.XPath(avatar.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public override Result ClickStartAdventure(int x, int y)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }

            _result = Click(By.XPath(finishButton.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = Wait(driver =>
            {
                var elements = driver.FindElements(By.Id("start"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = Click(By.Id("start"), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = Wait(driver =>
            {
                var elements = driver.FindElements(By.Id("ok"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public override Result ClickStartFarm(int farmId)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var chrome = _chromeBrowser.GetChrome();
            chrome.ExecuteScript($"Travian.Game.RaidList.toggleList({farmId});");
            DelayClick();

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);

                var waitFarmNode = waitHtml.GetElementbyId($"list{farmId}");
                var table = waitFarmNode.Descendants("div").FirstOrDefault(x => x.HasClass("listContent"));
                return !table.HasClass("hide");
            });

            _result = Click(By.Id($"raidListMarkAll{farmId}"));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            var html = _chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"list{farmId}");
            var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));

            if (buttonStartFarm is null)
            {
                return Result.Fail(new Retry("Cannot find button start farmlist"));
            }

            _result = Click(By.XPath(buttonStartFarm.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = SwitchTab(1);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}