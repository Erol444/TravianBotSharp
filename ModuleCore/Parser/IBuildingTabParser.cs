using HtmlAgilityPack;
using System.Collections.Generic;

namespace ModuleCore.Parser
{
    public interface IBuildingTabParser
    {
        public List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc);

        public bool IsCurrentTab(HtmlNode tabNode);
    }
}