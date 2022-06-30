using HtmlAgilityPack;
using System.Linq;

namespace TravianOffcialCore.FindElements
{
    public static class LoginPage
    {
        public static HtmlNode GetUsernameNode(HtmlDocument html)
        {
            return html.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("name"));
        }

        public static HtmlNode GetPasswordNode(HtmlDocument html)
        {
            return html.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("password"));
        }

        public static HtmlNode GetLoginButton(HtmlDocument html)
        {
            var trNode = html.DocumentNode.Descendants("tr").FirstOrDefault(x => x.HasClass("loginButtonRow"));
            if (trNode == null) return null;
            return trNode.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        }
    }
}