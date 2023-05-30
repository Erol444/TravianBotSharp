using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class HeroResourcesHelper : Base.HeroResourcesHelper
    {
        public HeroResourcesHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory) : base(chromeManager, heroSectionParser, generalHelper, contextFactory)
        {
        }

        protected override Result ClickItem(HeroItemEnums item)
        {
            var doc = _chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetItemSlot(doc, (int)item);
            if (node is null)
            {
                return Result.Fail($"Cannot find item {item}");
            }

            _result = _generalHelper.Click(_accountId, By.XPath(node.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(_accountId, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });

            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        protected override Result Confirm()
        {
            var doc = _chromeBrowser.GetHtml();
            var confirmButton = _heroSectionParser.GetConfirmButton(doc);
            if (confirmButton is null)
            {
                return Result.Fail("Cannot find confirm button");
            }

            _result = _generalHelper.Click(_accountId, By.XPath(confirmButton.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            _result = _generalHelper.Wait(_accountId, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });
            return Result.Ok();
        }
    }
}