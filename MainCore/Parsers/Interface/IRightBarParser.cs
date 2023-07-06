using HtmlAgilityPack;

namespace MainCore.Parsers.Interface
{
    public interface IRightBarParser
    {
        public bool HasPlusAccount(HtmlDocument doc);

        public int GetTribe(HtmlDocument doc);
    }
}