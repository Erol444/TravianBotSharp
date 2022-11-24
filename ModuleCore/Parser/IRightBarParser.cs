using HtmlAgilityPack;

namespace ModuleCore.Parser
{
    public interface IRightBarParser
    {
        public bool HasPlusAccount(HtmlDocument doc);

        public int GetTribe(HtmlDocument doc);
    }
}