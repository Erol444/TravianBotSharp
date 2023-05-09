using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class BuildingsHelper : Base.BuildingsHelper
    {
        public BuildingsHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager) : base(contextFactory, planManager, chromeManager)
        {
        }
    }
}