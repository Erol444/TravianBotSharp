using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public static class FarmList
    {
        public static List<HtmlNode> GetFarmNodes(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return new();
            var fls = raidList.Descendants("div").Where(x => x.HasClass("raidList"));

            return fls.ToList();
        }

        public static string GetName(HtmlNode node)
        {
            var flName = node.Descendants("div").FirstOrDefault(x => x.HasClass("listName"));
            if (flName is null) return null;
            return flName.InnerText.Trim();
        }

        public static int GetId(HtmlNode node)
        {
            var id = node.GetAttributeValue("data-listid", "0");
            var value = new string(id.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        public static int GetNumOfFarms(HtmlNode node)
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