using FluentResults;
using HtmlAgilityPack;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class CheckHelper : ICheckHelper
    {
        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly IBuildingTabParser _buildingTabParser;
        protected readonly ISystemPageParser _systemPageParser;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public CheckHelper(IChromeManager chromeManager, IVillagesTableParser villagesTableParser, IBuildingTabParser buildingTabParser, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser)
        {
            _chromeManager = chromeManager;
            _villagesTableParser = villagesTableParser;
            _buildingTabParser = buildingTabParser;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
        }

        public bool IsCorrectVillage()
        {
            var html = _chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            foreach (var node in listNode)
            {
                var id = _villagesTableParser.GetId(node);
                if (id != _villageId) continue;
                return _villagesTableParser.IsActive(node);
            }
            return false;
        }

        public int GetCurrentVillageId()
        {
            var html = _chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            foreach (var node in listNode)
            {
                if (!_villagesTableParser.IsActive(node)) continue;
                return _villagesTableParser.GetId(node);
            }
            return -1;
        }

        public bool IsCorrectTab(int tab)
        {
            var html = _chromeBrowser.GetHtml();
            var tabs = _buildingTabParser.GetBuildingTabNodes(html);
            return _buildingTabParser.IsCurrentTab(tabs[tab]);
        }

        public abstract bool IsFarmListPage();

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