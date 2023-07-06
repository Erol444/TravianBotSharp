using MainCore.Parsers.Interface;
using MainCore.Services.Interface;

namespace MainCore.Helper.Implementations.TTWars
{
    public class CheckHelper : Base.CheckHelper
    {
        public CheckHelper(IChromeManager chromeManager, IVillagesTableParser villagesTableParser, IBuildingTabParser buildingTabParser, ISystemPageParser systemPageParser) : base(chromeManager, villagesTableParser, buildingTabParser, systemPageParser)
        {
        }

        public override bool IsFarmListPage(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var url = chromeBrowser.GetCurrentUrl();
            if (!url.Contains("tt=99")) return false;
            return IsCorrectTab(accountId, 4);
        }
    }
}