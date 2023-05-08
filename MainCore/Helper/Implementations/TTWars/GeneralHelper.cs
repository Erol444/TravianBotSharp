using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;

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
                return Result.Fail(Retry.Msg("Cannot find adventures button"));
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
                return Result.Fail(Retry.Msg("Cannot find hero avatar"));
            }

            _result = Click(By.XPath(avatar.XPath));
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}