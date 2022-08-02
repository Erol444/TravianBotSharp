using HtmlAgilityPack;
using System.Linq;
using System.Net;

namespace TTWarsCore.Parsers
{
    public static class StockBar
    {
        public static long GetWood(HtmlDocument doc)
        {
            var woodNode = doc.GetElementbyId("l1");
            if (woodNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(woodNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static long GetClay(HtmlDocument doc)
        {
            var clayNode = doc.GetElementbyId("l2");
            if (clayNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(clayNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static long GetIron(HtmlDocument doc)
        {
            var ironNode = doc.GetElementbyId("l3");
            if (ironNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(ironNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static long GetCrop(HtmlDocument doc)
        {
            var cropNode = doc.GetElementbyId("l4");
            if (cropNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(cropNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static long GetFreeCrop(HtmlDocument doc)
        {
            var freecropNode = doc.GetElementbyId("stockBarFreeCrop");
            if (freecropNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(freecropNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static long GetWarehouseCapacity(HtmlDocument doc)
        {
            var warehousepNode = doc.GetElementbyId("stockBarWarehouse");
            if (warehousepNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(warehousepNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static long GetGranaryCapacity(HtmlDocument doc)
        {
            var granaryNode = doc.GetElementbyId("stockBarGranary");
            if (granaryNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(granaryNode.InnerText);
            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return long.Parse(valueStr);
        }

        public static int GetGold(HtmlDocument doc)
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

        public static int GetSilver(HtmlDocument doc)
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