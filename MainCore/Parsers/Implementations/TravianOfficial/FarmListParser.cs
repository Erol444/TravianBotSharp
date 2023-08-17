using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TravianOfficial
{
    public class FarmListParser : IFarmListParser
    {
        public HtmlNode GetStartButton(HtmlDocument doc, int raidId)
        {
            var farmNode = doc.GetElementbyId($"raidList{raidId}");
            if (farmNode is null) return null;
            var startNode = farmNode.Descendants("button")
                                    .FirstOrDefault(x => x.HasClass("startButton"));
            return startNode;
        }

        public List<HtmlNode> GetFarmNodes(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return new();
            var fls = raidList.Descendants("div").Where(x => x.HasClass("raidList"));

            return fls.ToList();
        }

        public string GetName(HtmlNode node)
        {
            var flName = node.Descendants("div").FirstOrDefault(x => x.HasClass("listName"));
            if (flName is null) return null;
            return flName.InnerText.Trim();
        }

        public int GetId(HtmlNode node)
        {
            var id = node.GetAttributeValue("data-listid", "0");
            var value = new string(id.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        public int GetNumOfFarms(HtmlNode node)
        {
            var slotCount = node.Descendants("span").FirstOrDefault(x => x.HasClass("slotsCount"));
            if (slotCount is null) return 0;
            var value = new string(slotCount.InnerText.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        public HtmlNode GetStartAllButton(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return null;
            var startAll = raidList.Descendants("button").FirstOrDefault(x => x.HasClass("startAll"));
            return startAll;
        }
    }
}