using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface IFarmListParser
    {
        public List<HtmlNode> GetFarmNodes(HtmlDocument doc);

        public string GetName(HtmlNode node);

        public int GetId(HtmlNode node);

        public int GetNumOfFarms(HtmlNode node);
    }
}