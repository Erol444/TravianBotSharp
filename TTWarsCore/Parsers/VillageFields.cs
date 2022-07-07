using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTWarsCore.Parsers
{
    public static class VillageFields
    {
        public static List<HtmlNode> GetResourceNodes(HtmlDocument doc)
        {
            var villageMapNode = doc.GetElementbyId("village_map");
            if (villageMapNode is null) return new();

            return villageMapNode.Descendants("div").Where(x => !x.HasClass("labelLayer")).ToList();
        }

        public static int GetResourceId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("aid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());

            return int.Parse(strResult);
        }

        public static int GetResourceType(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("gid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());

            return int.Parse(strResult);
        }

        public static int GetResourceLevel(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("level") && x != "level");
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());

            return int.Parse(strResult);
        }

        public static bool IsUnderConstruction(HtmlNode node)
        {
            return node.GetClasses().Contains("underConstruction");
        }
    }
}