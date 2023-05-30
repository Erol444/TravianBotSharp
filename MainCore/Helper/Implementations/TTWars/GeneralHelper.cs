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

        public GeneralHelper(IChromeManager chromeManager, INavigationBarParser navigationBarParser, ICheckHelper checkHelper, IVillagesTableParser villagesTableParser, IDbContextFactory<AppDbContext> contextFactory, IBuildingTabParser buildingTabParser, IUpdateHelper updateHelper, IInvalidPageHelper invalidPageHelper, IBuildingsHelper buildingsHelper, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IHeroSectionParser heroSectionParser) : base(chromeManager, navigationBarParser, checkHelper, villagesTableParser, contextFactory, buildingTabParser, updateHelper, invalidPageHelper)
        {
            _heroSectionParser = heroSectionParser;
        }

        public override Result ToBuilding(int accountId, int index)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var currentUrl = chromeBrowser.GetCurrentUrl();
            var uri = new Uri(currentUrl);
            var serverUrl = $"{uri.Scheme}://{uri.Host}";
            var url = $"{serverUrl}/build.php?id={index}";
            var result = Navigate(accountId, url);
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

            var result = Click(accountId, By.XPath(avatar.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _updateHelper.UpdateHeroInventory();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}