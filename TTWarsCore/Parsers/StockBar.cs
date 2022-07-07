using HtmlAgilityPack;
using System.Linq;
using System.Net;

namespace TTWarsCore.Parsers
{
    public static class StockBar
    {
        public static int GetWood(HtmlDocument doc)
        {
            var woodNode = doc.GetElementbyId("l1");
            if (woodNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(woodNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static int GetClay(HtmlDocument doc)
        {
            var clayNode = doc.GetElementbyId("l2");
            if (clayNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(clayNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static int GetIron(HtmlDocument doc)
        {
            var ironNode = doc.GetElementbyId("l3");
            if (ironNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(ironNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static int GetCrop(HtmlDocument doc)
        {
            var cropNode = doc.GetElementbyId("l4");
            if (cropNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(cropNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static int GetFreeCrop(HtmlDocument doc)
        {
            var freecropNode = doc.GetElementbyId("stockBarFreeCrop");
            if (freecropNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(freecropNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static int GetWarehouseCapacity(HtmlDocument doc)
        {
            var warehousepNode = doc.GetElementbyId("stockBarWarehouse");
            if (warehousepNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(warehousepNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }

        public static int GetGranaryCapacity(HtmlDocument doc)
        {
            var granaryNode = doc.GetElementbyId("stockBarGranary");
            if (granaryNode is null) return -1;
            var valueStrFixed = WebUtility.HtmlDecode(granaryNode.InnerText);
            var valueStr = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(valueStr);
        }
    }
}