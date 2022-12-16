using HtmlAgilityPack;
using ModuleCore.Parser;
using System.Collections.Generic;
using System.Linq;

namespace TTWarsCore.Parsers
{
    public class FarmListParser : IFarmListParser
    {
        public List<HtmlNode> GetFarmNodes(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return new();
            var fls = raidList.ChildNodes.Where(x => x.Id.StartsWith("list"));

            return fls.ToList();
        }

        public string GetName(HtmlNode node)
        {
            var flName = node.Descendants("div").FirstOrDefault(x => x.HasClass("listTitleText"));
            if (flName is null) return null;
            return flName.InnerText.Trim();
        }

        public int GetId(HtmlNode node)
        {
            var id = node.Id;
            var value = new string(id.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        public int GetNumOfFarms(HtmlNode node)
        {
            var slotCount = node.Descendants("span").FirstOrDefault(x => x.HasClass("raidListSlotCount"));
            if (slotCount is null) return 0;
            var slot = slotCount.InnerText.Split('/');
            if (slot.Length < 1) return 0;
            var value = new string(slot[0].Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }
    }
}