using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public static class BuildingTab
    {
        public static List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc)
        {
            var navigationBar = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
            var navNodes = navigationBar.Descendants("a").Where(x => x.HasClass("tabItem"));
            return navNodes.ToList();
        }

        public static bool IsCurrentTab(HtmlNode tabNode)
        {
            return tabNode.HasClass("active");
        }
    }
}