using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface ISubTabParser
    {
        public List<HtmlNode> GetProductions(HtmlDocument doc);

        public long GetProduction(HtmlNode node);
    }
}