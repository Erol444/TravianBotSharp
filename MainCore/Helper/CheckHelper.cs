using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Services;
using System.Linq;

#if TRAVIAN_OFFICIAL

using System;
using MainCore.Models.Runtime;
using TravianOfficialCore.Parsers;
using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using System;
using MainCore.Models.Runtime;
using TravianOfficialNewHeroUICore.Parsers;
using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.Parsers;
using TTWarsCore.FindElements;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Helper
{
    public static class CheckHelper
    {
        public static bool IsCorrectVillage(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.GetHtml();

            var listNode = VillagesTable.GetVillageNodes(html);
            foreach (var node in listNode)
            {
                var id = VillagesTable.GetId(node);
                if (id != villageId) continue;
                return VillagesTable.IsActive(node);
            }
            return false;
        }

        public static int GetCurrentVillageId(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();

            var listNode = VillagesTable.GetVillageNodes(html);
            foreach (var node in listNode)
            {
                if (!VillagesTable.IsActive(node)) continue;
                return VillagesTable.GetId(node);
            }
            return -1;
        }

        public static bool IsCorrectTab(IChromeBrowser chromeBrowser, int tab)
        {
            var html = chromeBrowser.GetHtml();
            var tabs = BuildingTab.GetBuildingTabNodes(html);
            return BuildingTab.IsCurrentTab(tabs[tab]);
        }

        public static int[] GetResourceNeed(IChromeBrowser chromeBrowser, BuildingEnums building, bool multiple = false)
        {
            var html = chromeBrowser.GetHtml();

            HtmlNode contractNode;
            if (multiple && !building.IsResourceField())
            {
                contractNode = html.GetElementbyId($"contract_building{(int)building}");
            }
            else
            {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                contractNode = html.GetElementbyId("contract");
#elif TTWARS
                contractNode = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.Id.Equals("contract"));
#else

#error You forgot to define Travian version here

#endif
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

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

        public static bool IsNeedAdsUpgrade(IChromeBrowser chromeBrowser, AppDbContext context, int villageId, PlanTask buildingTask)
        {
            var setting = context.VillagesSettings.Find(villageId);
            if (!setting.IsAdsUpgrade) return false;

            var building = context.VillagesBuildings.Find(villageId, buildingTask.Location);

            if (buildingTask.Building.IsResourceField() && building.Level == 0) return false;
            if (buildingTask.Building.IsNotAdsUpgrade()) return false;

            var html = chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("upgradeButtonsContainer"));

            var durationNode = container.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            if (durationNode is null)
            {
                throw new Exception("Cannot found duration in build page. (div)");
            }
            var dur = durationNode.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
            if (dur is null)
            {
                throw new Exception("Cannot found duration in build page. (span)");
            }
            var duration = dur.InnerText.ToDuration();
            if (setting.AdsUpgradeTime > duration.TotalMinutes) return false;
            return true;
        }

#elif TTWARS

#else

#error You forgot to define Travian version here

#endif

        public static bool IsFarmListPage(IChromeBrowser chromeBrowser)
        {
            // check building
            var url = chromeBrowser.GetCurrentUrl();
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            if (!url.Contains("id=39")) return false;
#elif TTWARS
            if (!url.Contains("tt=99")) return false;
#else

#error You forgot to define Travian version here

#endif
            //check tab
            return IsCorrectTab(chromeBrowser, 4);
        }

        public static bool IsWWMsg(HtmlDocument doc) => doc.DocumentNode.Descendants("img").FirstOrDefault(x => x.GetAttributeValue("src", "") == "/img/ww100.png") is not null;

        /// <summary>
        /// TTWars and other old server has ww img in natar profile so we need this one
        /// </summary>
        /// <param name="chromeBrowser"></param>
        /// <returns></returns>
        public static bool IsWWPage(IChromeBrowser chromeBrowser) => chromeBrowser.GetCurrentUrl().EndsWith("/spieler.php?uid=1");

        public static bool IsSkipTutorial(HtmlDocument doc) => doc.DocumentNode.Descendants().Any(x => x.HasClass("questButtonSkipTutorial"));

        public static bool IsContextualHelp(HtmlDocument doc) => doc.GetElementbyId("contextualHelp") is not null;

        public static bool IsBanMsg(HtmlDocument doc) => doc.GetElementbyId("punishmentMsgButtons") is not null;

        public static bool IsMaintanance(HtmlDocument doc) => doc.DocumentNode.Descendants("img").Any(x => x.HasClass("fatalErrorImage"));

        public static bool IsCaptcha(HtmlDocument doc) => doc.GetElementbyId("recaptchaImage") is not null;

        public static bool IsLoginScreen(HtmlDocument doc)
        {
            var username = LoginPage.GetUsernameNode(doc);
            var password = LoginPage.GetPasswordNode(doc);
            var button = LoginPage.GetLoginButton(doc);
            return username is not null && password is not null && button is not null;
        }

        public static bool IsSysMsg(HtmlDocument doc) => doc.GetElementbyId("sysmsg") is not null;
    }
}