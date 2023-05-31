using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class LoginHelper : Base.LoginHelper
    {
        public LoginHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser, IUpdateHelper updateHelper) : base(chromeManager, generalHelper, contextFactory, systemPageParser, updateHelper)
        {
        }
    }
}