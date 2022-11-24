using HtmlAgilityPack;

namespace ParserCore
{
    public interface IRightBarParser
    {
        public bool HasPlusAccount(HtmlDocument doc);

        public int GetTribe(HtmlDocument doc);
    }
}