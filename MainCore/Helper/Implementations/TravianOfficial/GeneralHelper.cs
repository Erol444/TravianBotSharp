using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class GeneralHelper : Base.GeneralHelper
    {
        private readonly IBuildingsHelper _buildingsHelper;
        private readonly IVillageFieldParser _villageFieldParser;
        private readonly IVillageInfrastructureParser _villageInfrastructureParser;
        private readonly IHeroSectionParser _heroSectionParser;

        public GeneralHelper(IChromeManager chromeManager, INavigationBarParser navigationBarParser, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, IDbContextFactory<AppDbContext> contextFactory, IBuildingTabParser buildingTabParser, IBuildingsHelper buildingsHelper, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IHeroSectionParser heroSectionParser) : base(chromeManager, navigationBarParser, checkHelper, villagesTableParser, contextFactory, buildingTabParser)
        {
            _buildingsHelper = buildingsHelper;
            _villageFieldParser = villageFieldParser;
            _villageInfrastructureParser = villageInfrastructureParser;
            _heroSectionParser = heroSectionParser;
        }

        public override Result ToBuilding(int index)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var dorf = _buildingsHelper.GetDorf(index);
            var chrome = _chromeBrowser.GetChrome();
            var html = _chromeBrowser.GetHtml();
            switch (dorf)
            {
                case 1:
                    {
                        var node = _villageFieldParser.GetNode(html, index);
                        if (node is null)
                        {
                            return Result.Fail(new Retry($"Cannot find resource field at {index}"));
                        }
                        _result = Click(By.XPath(node.XPath));
                        if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
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

                        _result = WaitPageChanged($"?id={index}");
                        if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    break;

                default:
                    break;
            }

            _result = WaitPageLoaded();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public override Result ToHeroInventory()
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();
            var avatar = _heroSectionParser.GetHeroAvatar(html);
            if (avatar is null)
            {
                return Result.Fail(new Retry("Cannot find hero avatar"));
            }
            _result = Click(By.XPath(avatar.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            var wait = _chromeBrowser.GetWait();
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

        public override Result ToAdventure()
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetAdventuresButton(html);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }
            _result = Click(By.XPath(node.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            var wait = _chromeBrowser.GetWait();
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

        public override Result ClickStartAdventure(int x, int y)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }

            _result = Click(By.XPath(finishButton.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public override Result ClickStartFarm(int farmId)
        {
            if (!IsPageValid()) return Result.Fail(Stop.Announcement);

            var html = _chromeBrowser.GetHtml();

            var farmNode = html.GetElementbyId($"raidList{farmId}");
            if (farmNode is null)
            {
                return Result.Fail(new Retry("Cannot found farm node"));
            }
            var startNode = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("startButton"));
            if (startNode is null)
            {
                return Result.Fail(new Retry("Cannot found start button"));
            }

            _result = Click(By.XPath(startNode.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}