using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class CheckHelper : ICheckHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly IBuildingTabParser _buildingTabParser;
        protected readonly ISystemPageParser _systemPageParser;

        public CheckHelper(IChromeManager chromeManager, IVillagesTableParser villagesTableParser, IBuildingTabParser buildingTabParser, ISystemPageParser systemPageParser)
        {
            _chromeManager = chromeManager;
            _villagesTableParser = villagesTableParser;
            _buildingTabParser = buildingTabParser;
            _systemPageParser = systemPageParser;
        }

        public abstract bool IsFarmListPage(int accountId);

        public Result<bool> IsCorrectVillage(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            foreach (var node in listNode)
            {
                var id = _villagesTableParser.GetId(node);
                if (id != villageId) continue;
                return _villagesTableParser.IsActive(node);
            }
            return Result.Fail(new Retry($"Not found villageId {villageId} in village list"));
        }

        public Result<int> GetCurrentVillageId(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            foreach (var node in listNode)
            {
                if (!_villagesTableParser.IsActive(node)) continue;
                return _villagesTableParser.GetId(node);
            }
            return Result.Fail(new Retry($"Cannot detect active village in village list"));
        }

        public bool IsCorrectTab(int accountId, int tab)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var tabs = _buildingTabParser.GetBuildingTabNodes(html);
            return _buildingTabParser.IsCurrentTab(tabs[tab]);
        }

        public bool IsLoginScreen(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var username = _systemPageParser.GetUsernameNode(html);
            var password = _systemPageParser.GetPasswordNode(html);
            var button = _systemPageParser.GetLoginButton(html);
            return username is not null && password is not null && button is not null;
        }
    }
}