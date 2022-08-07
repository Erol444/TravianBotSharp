using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace TTWarsCore.Parsers
{
    public static class BuildingTab
    {
        public static List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc)
        {
            var navigationBar = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
            var navNodes = navigationBar.Descendants("div").Where(x => x.HasClass("container"));
            return navNodes.ToList();
        }

        public static bool IsCurrentTab(HtmlNode tabNode)
        {
            return tabNode.HasClass("active");
        }
    }
}