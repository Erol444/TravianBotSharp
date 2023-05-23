using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class InvalidPageHelper : Base.InvalidPageHelper
    {
        public InvalidPageHelper(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser, INavigationBarParser navigationBarParser) : base(chromeManager, contextFactory, systemPageParser, navigationBarParser)
        {
        }
    }
}