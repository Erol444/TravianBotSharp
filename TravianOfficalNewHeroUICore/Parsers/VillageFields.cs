using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficalNewHeroUICore.Parsers
{
    public static class VillageFields
    {
        public static List<HtmlNode> GetResourceNodes(HtmlDocument doc)
        {
            var resourceFieldContainerNode = doc.GetElementbyId("resourceFieldContainer");
            if (resourceFieldContainerNode is null) return new();

            return resourceFieldContainerNode.ChildNodes.Where(x => x.HasClass("level")).ToList();
        }

        public static int GetResourceId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("buildingSlot"));
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