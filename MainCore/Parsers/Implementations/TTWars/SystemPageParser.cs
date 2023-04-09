using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System.Linq;

namespace MainCore.Parsers.Implementations.TTWars
{
    public class SystemPageParser : ISystemPageParser
    {
        public HtmlNode GetUsernameNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("name"));
        }

        public HtmlNode GetPasswordNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("password"));
        }

        public HtmlNode GetLoginButton(HtmlDocument doc)
        {
            return doc.GetElementbyId("s1");
        }

        public HtmlNode GetContractNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.Id.Equals("contract"));
        }

        public HtmlNode GetAdventuresDetail(HtmlDocument doc)
        {
            return doc.GetElementbyId("tileDetails");
        }

        public HtmlNode GetNpcSumNode(HtmlDocument doc)
        {
            return doc.GetElementbyId($"sum");
        }
    }
}