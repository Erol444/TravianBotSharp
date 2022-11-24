using HtmlAgilityPack;

namespace ServerModuleCore.Parser
{
    public interface IRightBarParser
    {
        public bool HasPlusAccount(HtmlDocument doc);

        public int GetTribe(HtmlDocument doc);
    }
}