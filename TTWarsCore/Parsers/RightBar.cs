using HtmlAgilityPack;
using System.Linq;

namespace TTWarsCore.Parsers
{
    public static class RightBar
    {
        public static bool? HasPlusAccount(HtmlDocument doc)
        {
            var buttons = doc.DocumentNode.Descendants("button");
            var off = buttons.FirstOrDefault(x => x.HasClass("barracksBlack"));
            if (off is not null) return false;

            var on = buttons.FirstOrDefault(x => x.HasClass("barracksWhite"));
            if (on is not null) return true;
            return null;
        }

        public static int GetTribe(HtmlDocument doc)
        {
            var nodeDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("playerName"));
            if (nodeDiv is null) return 0;

            var nodeImage = nodeDiv.Descendants("img").FirstOrDefault();
            if (nodeImage is null) return 0;

            var tribeStr = nodeImage.GetAttributeValue("alt", "");
            var valueStr = new string(tribeStr.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 0;
            return int.Parse(valueStr);
        }
    }
}