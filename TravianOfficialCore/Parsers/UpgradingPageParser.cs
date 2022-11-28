using HtmlAgilityPack;
using ServerModuleCore.Parser;

namespace TravianOfficialCore.Parsers
{
    public class UpgradingPageParser : IUpgradingPageParser
    {
        public HtmlNode GetContractNode(HtmlDocument doc)
        {
            return doc.GetElementbyId("contract");
        }
    }
}