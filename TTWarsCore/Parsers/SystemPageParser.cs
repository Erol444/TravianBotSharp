using HtmlAgilityPack;
using ServerModuleCore.Parser;
using System.Linq;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public class SystemPageParser : ISystemPageParser
    {
        public HtmlNode GetUsernameNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("user"));
        }

        public HtmlNode GetPasswordNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("pw"));
        }

        public HtmlNode GetLoginButton(HtmlDocument doc)
        {
            return doc.GetElementbyId("s1");
        }
    }
}