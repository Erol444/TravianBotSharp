using HtmlAgilityPack;
using ServerModuleCore.Parser;
using System;
using System.Linq;

namespace TTWarsCore.Parsers
{
    public class UpgradingPageParser : IUpgradingPageParser
    {
        public HtmlNode GetContractNode(HtmlDocument doc)
        {
            return html.DocumentNode.Descendants("div").FirstOrDefault(x => x.Id.Equals("contract"));
        }
    }
}