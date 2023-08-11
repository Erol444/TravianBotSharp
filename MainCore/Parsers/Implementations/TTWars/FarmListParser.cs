using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TTWars
{
    public class FarmListParser : IFarmListParser
    {
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
            var name = flName.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
            if (name is null) return null;
            return name.InnerText.Trim();
        }

        public int GetId(HtmlNode node)
        {
            var id = node.Id;
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