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

        public GeneralHelper(IChromeManager chromeManager, INavigationBarParser navigationBarParser, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, IDbContextFactory<AppDbContext> contextFactory, IBuildingTabParser buildingTabParser, IUpdateHelper updateHelper, IInvalidPageHelper invalidPageHelper, IBuildingsHelper buildingsHelper, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IHeroSectionParser heroSectionParser) : base(chromeManager, navigationBarParser, checkHelper, villagesTableParser, contextFactory, buildingTabParser, updateHelper, invalidPageHelper)
        {
            _buildingsHelper = buildingsHelper;
            _villageFieldParser = villageFieldParser;
            _villageInfrastructureParser = villageInfrastructureParser;
            _heroSectionParser = heroSectionParser;
        }

        public override Result ToBuilding(int accountId, int index)
        {
            var dorf = _buildingsHelper.GetDorf(index);
            var chromeBrowser = _chromeManager.Get(accountId);
            var chrome = chromeBrowser.GetChrome();
            var html = chromeBrowser.GetHtml();
            Result result;
            switch (dorf)
            {
                case 1:
                    {
                        result = ToDorf1(accountId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                        var node = _villageFieldParser.GetNode(html, index);
                        if (node is null)
                        {
                            return Result.Fail(new Retry($"Cannot find resource field at {index}"));
                        }
                        result = Click(accountId, By.XPath(node.XPath));
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    break;

                case 2:
                    {
                        result = ToDorf2(accountId);
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

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

                        result = WaitPageChanged(accountId, $"?id={index}");
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    break;

                default:
                    break;
            }

            result = WaitPageLoaded(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
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
            var result = Click(accountId, By.XPath(avatar.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = Wait(accountId, driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var tab = _heroSectionParser.GetHeroTab(doc, 1);
                if (tab is null) return false;
                return _heroSectionParser.IsCurrentTab(tab);
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _updateHelper.UpdateHeroInventory();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}