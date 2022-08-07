using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Services;
using System.Linq;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

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

        public static bool IsCorrectTab(IChromeBrowser chromeBrowser, int tab)
        {
            var tabs = BuildingTab.GetBuildingTabNodes(chromeBrowser.GetHtml());
            return BuildingTab.IsCurrentTab(tabs[tab]);
        }

        public static int[] GetResourceNeed(IChromeBrowser chromeBrowser, BuildingEnums building, bool multiple = false)
        {
            var html = chromeBrowser.GetHtml();
            HtmlNode contractNode;
            if (multiple)
            {
                contractNode = html.GetElementbyId($"contract_building{(int)building}");
            }
            else
            {
                contractNode = html.GetElementbyId("contract");
            }
            var resWrapper = contractNode.Descendants().FirstOrDefault(x => x.HasClass("resourceWrapper"));
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
    }
}