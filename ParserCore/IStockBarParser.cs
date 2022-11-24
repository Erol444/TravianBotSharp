using HtmlAgilityPack;

namespace ParserCore
{
    public interface IStockBarParser
    {
        public long GetWood(HtmlDocument doc);

        public long GetClay(HtmlDocument doc);

        public long GetIron(HtmlDocument doc);

        public long GetCrop(HtmlDocument doc);

        public long GetFreeCrop(HtmlDocument doc);

        public long GetWarehouseCapacity(HtmlDocument doc);

        public long GetGranaryCapacity(HtmlDocument doc);

        public int GetGold(HtmlDocument doc);

        public int GetSilver(HtmlDocument doc);
    }
}