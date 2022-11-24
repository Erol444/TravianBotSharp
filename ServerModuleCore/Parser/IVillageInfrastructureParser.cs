using HtmlAgilityPack;
using System.Collections.Generic;

namespace ServerModuleCore.Parser
{
    public interface IVillageInfrastructureParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc);

        /// <summary>
        /// For Travian Official only
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public HtmlNode GetNode(HtmlDocument doc, int index);

        public int GetId(HtmlNode node);

        public int GetBuildingType(HtmlNode node);

        public int GetLevel(HtmlNode node);

        public bool IsUnderConstruction(HtmlNode node);
    }
}