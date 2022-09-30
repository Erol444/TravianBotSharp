using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialCore.Parsers
{
    public static class VillageCurrentlyBuilding
    {
        public static List<HtmlNode> GetNodes(HtmlDocument doc)
        {
            var finishButton = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishButton is null) return new();
            return finishButton.ParentNode.Descendants("li").ToList();
        }

        public static string GetType(HtmlNode node)
        {
            var nodeName = node.Descendants("div").FirstOrDefault(x => x.HasClass("name"));
            if (nodeName is null) return "";

            return new string(nodeName.ChildNodes[0].InnerText.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
        }

        public static int GetLevel(HtmlNode node)
        {
            var nodeLevel = node.Descendants("span").FirstOrDefault(x => x.HasClass("lvl"));
            if (nodeLevel is null) return 0;

            return int.Parse(new string(nodeLevel.InnerText.Where(c => char.IsDigit(c)).ToArray()));
        }

        public static TimeSpan GetDuration(HtmlNode node)
        {
            var nodeTimer = node.Descendants().FirstOrDefault(x => x.HasClass("timer"));
            if (nodeTimer is null) return TimeSpan.Zero;
            var strSec = new string(nodeTimer.GetAttributeValue("value", "0").Where(c => char.IsNumber(c)).ToArray());
            int sec = int.Parse(strSec);
            if (sec < 0) sec = 0;
            return TimeSpan.FromSeconds(sec);
        }
    }
}