using HtmlAgilityPack;
using MainCore.Parser.Interface;
using System.Linq;

namespace MainCore.Parser.Implementations.TravianOfficial
{
    public class RightBarParser : IRightBarParser
    {
        public bool HasPlusAccount(HtmlDocument doc)
        {
            var market = doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("market") && x.HasClass("round"));
            if (market is null) return false;

            if (market.HasClass("green")) return true;
            if (market.HasClass("gold")) return false;
            return false;
        }

        public int GetTribe(HtmlDocument doc)
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