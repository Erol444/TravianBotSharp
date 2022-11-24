using HtmlAgilityPack;
using ServerModuleCore.Parser;
using System.Linq;
using System.Net;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public class StockBarParser : IStockBarParser
    {
        private static long GetResource(HtmlDocument doc, string id)
        {
            var node = doc.GetElementbyId(id);
            if (node is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(node.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public long GetWood(HtmlDocument doc) => GetResource(doc, "l1");

        public long GetClay(HtmlDocument doc) => GetResource(doc, "l2");

        public long GetIron(HtmlDocument doc) => GetResource(doc, "l3");

        public long GetCrop(HtmlDocument doc) => GetResource(doc, "l3");

        public long GetFreeCrop(HtmlDocument doc) => GetResource(doc, "stockBarFreeCrop");

        public long GetWarehouseCapacity(HtmlDocument doc) => GetResource(doc, "stockBarWarehouse");

        public long GetGranaryCapacity(HtmlDocument doc) => GetResource(doc, "stockBarGranary");

        public int GetGold(HtmlDocument doc)
        {
            var goldNode = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("gold"));
            if (goldNode is null) return -1;
            var goldChildNode = goldNode.ChildNodes.FirstOrDefault(x => x.Name == "span");
            if (goldChildNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(goldChildNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return int.Parse(valueStr);
        }

        public int GetSilver(HtmlDocument doc)
        {
            var silverNode = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("silver"));
            if (silverNode is null) return -1;
            var silverChildNode = silverNode.ChildNodes.FirstOrDefault(x => x.Name == "span");
            if (silverChildNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(silverChildNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return int.Parse(valueStr);
        }
    }
}