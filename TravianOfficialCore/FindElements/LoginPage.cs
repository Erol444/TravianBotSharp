using HtmlAgilityPack;
using System.Linq;

namespace TravianOfficialCore.FindElements
{
    public static class LoginPage
    {
        public static HtmlNode GetUsernameNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("name"));
        }

        public static HtmlNode GetPasswordNode(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("password"));
        }

        public static HtmlNode GetLoginButton(HtmlDocument doc)
        {
            var trNode = doc.DocumentNode.Descendants("tr").FirstOrDefault(x => x.HasClass("loginButtonRow"));
            if (trNode == null) return null;
            return trNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        }
    }
}