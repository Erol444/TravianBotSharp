using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialCore.Parsers
{
    public static class VillagesTable
    {
        public static List<HtmlNode> GetVillageNodes(HtmlDocument doc)
        {
            var villsNode = doc.GetElementbyId("sidebarBoxVillagelist");
            if (villsNode is null) return new();
            return villsNode.Descendants("div").Where(x => x.HasClass("listEntry")).ToList();
        }

        public static bool IsUnderAttack(HtmlNode node)
        {
            return node.HasClass("attack");
        }

        public static bool IsActive(HtmlNode node)
        {
            return node.HasClass("active");
        }

        public static int GetId(HtmlNode node)
        {
            var dataDid = node.GetAttributeValue("data-did", 0);
            return dataDid;
        }

        public static string GetName(HtmlNode node)
        {
            var textNode = node.Descendants("a").FirstOrDefault();
            if (textNode is null) return "";
            var nameNode = textNode.Descendants("span").FirstOrDefault(x => x.HasClass("name"));
            if (nameNode is null) return "";
            return nameNode.InnerText;
        }

        public static int GetX(HtmlNode node)
        {
            var xNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateX"));
            if (xNode is null) return 0;
            var xStr = new string(xNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('−')).ToArray());
            xStr = xStr.Replace('−', '-');
            if (string.IsNullOrEmpty(xStr)) return 0;

            return int.Parse(xStr);
        }

        public static int GetY(HtmlNode node)
        {
            var yNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateY"));
            if (yNode is null) return 0;
            var yStr = new string(yNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('−')).ToArray());
            yStr = yStr.Replace('−', '-');
            if (string.IsNullOrEmpty(yStr)) return 0;
            return int.Parse(yStr);
        }
    }
}