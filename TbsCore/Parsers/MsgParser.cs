using HtmlAgilityPack;
using System.Linq;

namespace TbsCore.Parsers
{
    public static class MsgParser
    {
        public static int UnreadMessages(HtmlDocument htmlDoc)
        {
            var msgs5 = htmlDoc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("messages"));
            if (msgs5 == null) return 0;
            var indicator = msgs5.Descendants("div").FirstOrDefault(x => x.HasClass("indicator"));
            if (indicator == null) return 0;
            return (int)Parser.RemoveNonNumeric(indicator.InnerHtml);
        }
    }
}