﻿using FluentResults;
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

        public override Result ClickItem(int accountId, HeroItemEnums item)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetItemSlot(doc, (int)item);
            if (node is null)
            {
                return Result.Fail($"Cannot find item {item}");
            }

            var result = _generalHelper.Click(accountId, By.XPath(node.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(accountId, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });

            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public override Result Confirm(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var confirmButton = _heroSectionParser.GetConfirmButton(doc);
            if (confirmButton is null)
            {
                return Result.Fail("Cannot find confirm button");
            }

            var result = _generalHelper.Click(accountId, By.XPath(confirmButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = _generalHelper.Wait(accountId, driver =>
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