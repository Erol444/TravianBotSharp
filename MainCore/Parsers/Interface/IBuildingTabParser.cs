using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface IBuildingTabParser
    {
        public List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc);

        public bool IsCurrentTab(HtmlNode tabNode);
    }
}