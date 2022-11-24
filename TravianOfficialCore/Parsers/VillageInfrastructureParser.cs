using HtmlAgilityPack;
using ParserCore;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialCore.Parsers
{
    public class VillageInfrastructureParser : IVillageInfrastructureParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc)
        {
            var villageContentNode = doc.GetElementbyId("villageContent");
            if (villageContentNode is null) return new();
            var list = villageContentNode.Descendants("div").Where(x => x.HasClass("buildingSlot")).ToList();
            if (list.Count == 23) // level 1 wall and above has 2 part
            {
                list.RemoveAt(list.Count - 1);
            }

            return list;
        }

        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            var location = index - 18; // - 19 + 1
            return doc.DocumentNode.SelectSingleNode($"//*[@id='villageContent']/div[{location}]");
        }

        public int GetId(HtmlNode node)
        {
            return node.GetAttributeValue<int>("data-aid", -1);
        }

        public int GetBuildingType(HtmlNode node)
        {
            return node.GetAttributeValue<int>("data-gid", -1);
        }

        public int GetLevel(HtmlNode node)
        {
            var aNode = node.Descendants("a").FirstOrDefault();
            if (aNode is null) return -1;
            return aNode.GetAttributeValue<int>("data-level", -1);
        }

        public bool IsUnderConstruction(HtmlNode node)
        {
            return node.Descendants("a").Any(x => x.HasClass("underConstruction"));
        }
    }
}