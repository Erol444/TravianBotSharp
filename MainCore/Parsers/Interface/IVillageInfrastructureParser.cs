using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parser.Interface
{
    public interface IVillageInfrastructureParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc);

        public HtmlNode GetNode(HtmlDocument doc, int index);

        public int GetId(HtmlNode node);

        public int GetBuildingType(HtmlNode node);

        public int GetLevel(HtmlNode node);

        public bool IsUnderConstruction(HtmlNode node);
    }
}