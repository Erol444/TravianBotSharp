using HtmlAgilityPack;
using System.Linq;

namespace TravianOfficialCore.Parsers
{
    public static class RightBar
    {
        public static bool? HasPlusAccount(HtmlDocument doc)
        {
            var market = doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("market") && x.HasClass("round"));
            if (market is null) return null;

            if (market.HasClass("green")) return true;
            if (market.HasClass("gold")) return false;
            return null;
        }

        public static int GetTribe(HtmlDocument doc)
        {
            var questMaster = doc.GetElementbyId("questmasterButton");
            if (questMaster is null) return 0;

            var vid = questMaster.GetClasses().FirstOrDefault(x => x.StartsWith("vid"));
            if (vid is null) return 0;

            var valueStr = new string(vid.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 0;

            return int.Parse(valueStr);
        }
    }
}