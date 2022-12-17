using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ModuleCore.Parser;
using System.Linq;

namespace MainCore.Helper.Implementations
{
    public class CheckHelper : ICheckHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IVillagesTableParser _villagesTableParser;
        private readonly IBuildingTabParser _buildingTabParser;
        private readonly ISystemPageParser _systemPageParser;

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

        public int[] GetResourceNeed(int accountId, BuildingEnums building, bool multiple = false)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            HtmlNode contractNode;
            if (multiple && !building.IsResourceField())
            {
                contractNode = html.GetElementbyId($"contract_building{(int)building}");
            }
            else
            {
                contractNode = _systemPageParser.GetContractNode(html);
            }
            var resWrapper = contractNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var resNodes = resWrapper.ChildNodes.Where(x => x.HasClass("resource") || x.HasClass("resources")).ToList();
            var resNeed = new int[4];
            for (var i = 0; i < 4; i++)
            {
                var node = resNodes[i];
                var strResult = new string(node.InnerText.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(strResult)) resNeed[i] = 0;
                else resNeed[i] = int.Parse(strResult);
            }
            return resNeed;
        }

        public Result<bool> IsNeedAdsUpgrade(int accountId, int villageId, PlanTask buildingTask)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(villageId);
            if (!setting.IsAdsUpgrade) return false;

            var building = context.VillagesBuildings.Find(villageId, buildingTask.Location);

            if (buildingTask.Building.IsResourceField() && building.Level == 0) return false;
            if (buildingTask.Building.IsNotAdsUpgrade()) return false;

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));

            var durationNode = container.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            if (durationNode is null)
            {
                return Result.Fail(new MustRetry("Cannot found duration in build page. (div)"));
            }
            var dur = durationNode.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
            if (dur is null)
            {
                return Result.Fail(new MustRetry("Cannot found duration in build page. (span)"));
            }
            var duration = dur.InnerText.ToDuration();
            if (setting.AdsUpgradeTime > duration.TotalMinutes) return false;
            return true;
        }

        public bool IsFarmListPage(int accountId)
        {
            // check building
            var chromeBrowser = _chromeManager.Get(accountId);
            var url = chromeBrowser.GetCurrentUrl();

            if (VersionDetector.IsTravianOfficial() && !url.Contains("id=39")) return false;
            if (VersionDetector.IsTTWars() && !url.Contains("tt=99")) return false;
            return IsCorrectTab(accountId, 4);
        }

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