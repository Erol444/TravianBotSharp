using MainCore.Helper.Interface;
using MainCore.Parser.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TTWars
{
    public class UpgradeBuildingHelper : Base.UpgradeBuildingHelper
    {
        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IBuildingsHelper buildingsHelper, INavigateHelper navigateHelper, ILogManager logManager) : base(contextFactory, planManager, chromeManager, systemPageParser, buildingsHelper, navigateHelper, logManager)
        {
        }
    }
}