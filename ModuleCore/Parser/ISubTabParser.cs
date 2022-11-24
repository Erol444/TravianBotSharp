using HtmlAgilityPack;
using System.Collections.Generic;

namespace ModuleCore.Parser
{
    public interface ISubTabParser
    {
        public List<HtmlNode> GetProductions(HtmlDocument doc);

        public long GetProduction(HtmlNode node);
    }
}