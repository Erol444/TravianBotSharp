using HtmlAgilityPack;
using System.Linq;

namespace TTWarsCore.FindElements
{
    public static class LoginPage
    {
        public static HtmlNode GetUsernameNode(HtmlDocument html)
        {
            return html.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("user"));
        }

        public static HtmlNode GetPasswordNode(HtmlDocument html)
        {
            return html.DocumentNode.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "").Equals("pw"));
        }

        public static HtmlNode GetLoginButton(HtmlDocument html)
        {
            return html.GetElementbyId("s1");
        }
    }
}