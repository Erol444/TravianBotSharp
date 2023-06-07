using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class UpgradeBuildingHelper : Base.UpgradeBuildingHelper
    {
        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IGeneralHelper generalHelper, IEventManager eventManager, IHeroResourcesHelper heroResourcesHelper) : base(contextFactory, planManager, chromeManager, systemPageParser, generalHelper, eventManager, heroResourcesHelper)
        {
        }

        public override DateTime GetNextExecute(DateTime completeTime)
        {
            var length = DateTime.Now - completeTime;
            return completeTime.Add(length / 100);
        }
    }
}