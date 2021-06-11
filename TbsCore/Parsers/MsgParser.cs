using System.Linq;
using TbsCore.Helpers;

namespace TbsCore.Parsers
{
    public static class MsgParser
    {
        public static int UnreadMessages(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var msgs = htmlDoc.GetElementbyId("n6");
                    var container = msgs.Descendants("div").FirstOrDefault(x => x.HasClass("speechBubbleContainer"));
                    if (container == null) return 0;
                    var msgCount = container.Descendants("div").FirstOrDefault(x => x.HasClass("speechBubbleContent")).InnerHtml;
                    return (int)Parser.RemoveNonNumeric(msgCount);

                case Classificator.ServerVersionEnum.T4_5:
                    var msgs5 = htmlDoc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("messages"));
                    if (msgs5 == null) return 0;
                    var indicator = msgs5.Descendants("div").FirstOrDefault(x => x.HasClass("indicator"));
                    if (indicator == null) return 0;
                    return (int)Parser.RemoveNonNumeric(indicator.InnerHtml);
            }
            return 0;
        }
    }
}