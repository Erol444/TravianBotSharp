using HtmlAgilityPack;
using System.Collections.Generic;

namespace ServerModuleCore.Parser
{
    public interface IBuildingTabParser
    {
        public List<HtmlNode> GetBuildingTabNodes(HtmlDocument doc);

        public bool IsCurrentTab(HtmlNode tabNode);
    }
}