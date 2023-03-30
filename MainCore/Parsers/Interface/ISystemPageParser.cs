using HtmlAgilityPack;

namespace MainCore.Parser.Interface
{
    public interface ISystemPageParser
    {
        public HtmlNode GetUsernameNode(HtmlDocument doc);

        public HtmlNode GetPasswordNode(HtmlDocument doc);

        public HtmlNode GetLoginButton(HtmlDocument doc);

        public HtmlNode GetContractNode(HtmlDocument doc);

        public HtmlNode GetAdventuresDetail(HtmlDocument doc);

        public HtmlNode GetNpcSumNode(HtmlDocument doc);
    }
}