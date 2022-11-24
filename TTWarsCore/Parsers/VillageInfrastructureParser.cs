using HtmlAgilityPack;
using ServerModuleCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public class VillageInfrastructureParser : IVillageInfrastructureParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc)
        {
            var villageContentNode = doc.GetElementbyId("village_map");
            if (villageContentNode is null) return new();
            var list = villageContentNode.Descendants("div").Where(x => x.HasClass("buildingSlot")).ToList();
            if (list.Count == 22) return list;
            list.RemoveAt(22);
            return list;
        }

        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            throw new NotSupportedException();
        }

        public int GetId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("a"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;
            return int.Parse(strResult);
        }

        public int GetBuildingType(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("g"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;

            return int.Parse(strResult);
        }

        public int GetLevel(HtmlNode node)
        {
            var labelLayerNode = node.Descendants("div").FirstOrDefault(x => x.HasClass("labelLayer"));
            if (labelLayerNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(labelLayerNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return int.Parse(valueStr);
        }

        public bool IsUnderConstruction(HtmlNode node)
        {
            return node.Descendants("div").Any(x => x.HasClass("underConstruction"));
        }
    }
}