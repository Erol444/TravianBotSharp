using HtmlAgilityPack;
using ParserCore;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public class BuildingTabParser : IBuildingTabParser
    {
        public List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc)
        {
            var navigationBar = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
            var navNodes = navigationBar.Descendants("a").Where(x => x.HasClass("tabItem"));
            return navNodes.ToList();
        }

        public bool IsCurrentTab(HtmlNode tabNode)
        {
            return tabNode.HasClass("active");
        }
    }
}