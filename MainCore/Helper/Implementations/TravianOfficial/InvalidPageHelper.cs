using MainCore.Parsers.Interface;
using MainCore.Services.Interface;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class InvalidPageHelper : Base.InvalidPageHelper
    {
        public InvalidPageHelper(IChromeManager chromeManager, ISystemPageParser systemPageParser, INavigationBarParser navigationBarParser) : base(chromeManager, systemPageParser, navigationBarParser)
        {
        }
    }
}