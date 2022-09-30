using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public static class SubTab
    {
        public static List<HtmlNode> GetProductionNum(HtmlDocument doc)
        {
            var table = doc.GetElementbyId("production");
            if (table is null) return null;
            return table.Descendants("td").Where(x => x.HasClass("num")).ToList();
        }

        public static long GetNum(HtmlNode node)
        {
            var valueStrFixed = WebUtility.HtmlDecode(node.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }
    }
}