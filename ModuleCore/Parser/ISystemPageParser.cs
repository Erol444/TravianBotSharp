using HtmlAgilityPack;

namespace ModuleCore.Parser
{
    public interface ISystemPageParser
    {
        public HtmlNode GetUsernameNode(HtmlDocument doc);

        public HtmlNode GetPasswordNode(HtmlDocument doc);

        public HtmlNode GetLoginButton(HtmlDocument doc);
    }
}