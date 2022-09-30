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

        public static int GetId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("aid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;
            return int.Parse(strResult);
        }

        public static int GetType(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("gid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;

            return int.Parse(strResult);
        }

        public static int GetLevel(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("level") && !x.Equals("level"));
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