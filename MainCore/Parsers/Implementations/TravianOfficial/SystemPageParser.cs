using HtmlAgilityPack;
using MainCore.Parser.Interface;
using System.Linq;

namespace MainCore.Parser.Implementations.TravianOfficial
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
            var trNode = doc.DocumentNode.Descendants("tr").FirstOrDefault(x => x.HasClass("loginButtonRow"));
            if (trNode == null) return null;
            return trNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        }

        public HtmlNode GetContractNode(HtmlDocument doc)
        {
            return doc.GetElementbyId("contract");
        }

        public HtmlNode GetAdventuresDetail(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroAdventure");
        }

        public HtmlNode GetNpcSumNode(HtmlDocument doc)
        {
            return doc.GetElementbyId($"sum");
        }
    }
}