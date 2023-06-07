using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace MainCore.Helper.Implementations.TTWars
{
    public class UpgradeBuildingHelper : Base.UpgradeBuildingHelper
    {
        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IGeneralHelper generalHelper, IEventManager eventManager, IHeroResourcesHelper heroResourcesHelper) : base(contextFactory, planManager, chromeManager, systemPageParser, generalHelper, eventManager, heroResourcesHelper)
        {
        }

        public override DateTime GetNextExecute(DateTime completeTime)
        {
            return completeTime.AddSeconds(1);
        }
    }
}