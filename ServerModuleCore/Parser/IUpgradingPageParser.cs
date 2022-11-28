using HtmlAgilityPack;

namespace ServerModuleCore.Parser
{
    public interface IUpgradingPageParser
    {
        public HtmlNode GetContractNode(HtmlDocument doc);
    }
}