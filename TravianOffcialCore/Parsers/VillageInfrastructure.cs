using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TravianOffcialCore.Parsers
{
    public class VillageInfrastructure
    {
        public static List<HtmlNode> GetBuildingNodes(HtmlDocument doc)
        {
            var villageContentNode = doc.GetElementbyId("villageContent");
            if (villageContentNode is null) return new();
            return villageContentNode.Descendants("div").ToList();
        }

        public static int GetId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("aid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());

            return int.Parse(strResult);
        }

        public static int GetType(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("gid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());

            return int.Parse(strResult);
        }

        public static int GetLevel(HtmlNode node)
        {
            var labelLayerNode = node.Descendants("div").FirstOrDefault(x => x.HasClass("labelLayer"));
            if (labelLayerNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(labelLayerNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static bool IsUnderConstruction(HtmlNode node)
        {
            return node.Descendants("a").Any(x => x.HasClass("underConstruction"));
        }
    }
}