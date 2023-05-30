using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class CompleteNowHelper : Base.CompleteNowHelper
    {
        public CompleteNowHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IGeneralHelper generalHelper, IChromeManager chromeManager) : base(villageCurrentlyBuildingParser, generalHelper, chromeManager)
        {
        }
    }
}