using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.TTWars
{
    public class HeroResourcesHelper : Base.HeroResourcesHelper
    {
        public HeroResourcesHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper) : base(chromeManager, heroSectionParser, generalHelper)
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

            _result = _generalHelper.Click(By.XPath(node.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var dialog = _heroSectionParser.GetAmountBox(html);
                return dialog is not null;
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
                return Result.Fail(Retry.ButtonNotFound("confirm"));
            }

            _result = _generalHelper.Click(By.XPath(confirmButton.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}