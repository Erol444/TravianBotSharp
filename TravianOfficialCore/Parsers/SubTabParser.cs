using HtmlAgilityPack;
using ModuleCore.Parser;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TravianOfficialCore.Parsers
{
    public class SubTabParser : ISubTabParser
    {
        public List<HtmlNode> GetProductions(HtmlDocument doc)
        {
            var table = doc.GetElementbyId("production");
            if (table is null) return null;
            return table.Descendants("td").Where(x => x.HasClass("num")).ToList();
        }

        public long GetProduction(HtmlNode node)
        {
            var valueStrFixed = WebUtility.HtmlDecode(node.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }
    }
}