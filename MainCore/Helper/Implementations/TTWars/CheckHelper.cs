﻿using MainCore.Parser.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TTWars
{
    public class CheckHelper : Base.CheckHelper
    {
        public CheckHelper(IChromeManager chromeManager, IVillagesTableParser villagesTableParser, IBuildingTabParser buildingTabParser, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser) : base(chromeManager, villagesTableParser, buildingTabParser, contextFactory, systemPageParser)
        {
        }

        public override bool IsFarmListPage(int accountId)
        {
            // check building
            var chromeBrowser = _chromeManager.Get(accountId);
            var url = chromeBrowser.GetCurrentUrl();

            if (!url.Contains("tt=99")) return false;
            return IsCorrectTab(accountId, 4);
        }
    }
}