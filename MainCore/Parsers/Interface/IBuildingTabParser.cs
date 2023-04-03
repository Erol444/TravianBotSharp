using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parser.Interface
{
    public interface IBuildingTabParser
    {
        public List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc);

        public bool IsCurrentTab(HtmlNode tabNode);
    }
}