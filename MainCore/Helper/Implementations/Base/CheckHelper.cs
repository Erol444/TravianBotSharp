using HtmlAgilityPack;
using MainCore.Helper.Interface;
using MainCore.Parser.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class CheckHelper : ICheckHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly IBuildingTabParser _buildingTabParser;
        protected readonly ISystemPageParser _systemPageParser;

        public CheckHelper(IChromeManager chromeManager, IVillagesTableParser villagesTableParser, IBuildingTabParser buildingTabParser, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser)
        {
            _chromeManager = chromeManager;
            _villagesTableParser = villagesTableParser;
            _buildingTabParser = buildingTabParser;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
        }

        public bool IsCorrectVillage(int accountId, int villageId)
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
            return false;
        }

        public int GetCurrentVillageId(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            foreach (var node in listNode)
            {
                if (!_villagesTableParser.IsActive(node)) continue;
                return _villagesTableParser.GetId(node);
            }
            return -1;
        }

        public bool IsCorrectTab(int accountId, int tab)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var tabs = _buildingTabParser.GetBuildingTabNodes(html);
            return _buildingTabParser.IsCurrentTab(tabs[tab]);
        }

        public abstract bool IsFarmListPage(int accountId);

        public bool IsWWMsg(HtmlDocument doc) => doc.DocumentNode.Descendants("img").FirstOrDefault(x => x.GetAttributeValue("src", "") == "/img/ww100.png") is not null;

        public bool IsWWPage(IChromeBrowser chromeBrowser) => chromeBrowser.GetCurrentUrl().EndsWith("/spieler.php?uid=1");

        public bool IsSkipTutorial(HtmlDocument doc) => doc.DocumentNode.Descendants().Any(x => x.HasClass("questButtonSkipTutorial"));

        public bool IsContextualHelp(HtmlDocument doc) => doc.GetElementbyId("contextualHelp") is not null;

        public bool IsBanMsg(HtmlDocument doc) => doc.GetElementbyId("punishmentMsgButtons") is not null;

        public bool IsMaintanance(HtmlDocument doc) => doc.DocumentNode.Descendants("img").Any(x => x.HasClass("fatalErrorImage"));

        public bool IsCaptcha(HtmlDocument doc) => doc.GetElementbyId("recaptchaImage") is not null;

        public bool IsLoginScreen(HtmlDocument doc)
        {
            var username = _systemPageParser.GetUsernameNode(doc);
            var password = _systemPageParser.GetPasswordNode(doc);
            var button = _systemPageParser.GetLoginButton(doc);
            return username is not null && password is not null && button is not null;
        }

        public bool IsSysMsg(HtmlDocument doc) => doc.GetElementbyId("sysmsg") is not null;
    }
}