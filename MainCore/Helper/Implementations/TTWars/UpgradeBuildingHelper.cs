using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TTWars
{
    public class UpgradeBuildingHelper : Base.UpgradeBuildingHelper
    {
        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IBuildingsHelper buildingsHelper, IGeneralHelper generalHelper, ILogManager logManager, IEventManager eventManager, IHeroResourcesHelper heroResourcesHelper) : base(contextFactory, planManager, chromeManager, systemPageParser, buildingsHelper, generalHelper, logManager, eventManager, heroResourcesHelper)
        {
        }
    }
}