using HtmlAgilityPack;
using ModuleCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialCore.Parsers
{
    public class VillageCurrentlyBuildingParser : IVillageCurrentlyBuildingParser
    {
        public List<HtmlNode> GetItems(HtmlDocument doc)
        {
            var finishButton = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishButton is null) return new();
            return finishButton.ParentNode.Descendants("li").ToList();
        }

        public string GetBuildingType(HtmlNode node)
        {
            var nodeName = node.Descendants("div").FirstOrDefault(x => x.HasClass("name"));
            if (nodeName is null) return "";

            return new string(nodeName.ChildNodes[0].InnerText.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
        }

        public int GetLevel(HtmlNode node)
        {
            var nodeLevel = node.Descendants("span").FirstOrDefault(x => x.HasClass("lvl"));
            if (nodeLevel is null) return 0;

            return int.Parse(new string(nodeLevel.InnerText.Where(c => char.IsDigit(c)).ToArray()));
        }

        public TimeSpan GetDuration(HtmlNode node)
        {
            var nodeTimer = node.Descendants().FirstOrDefault(x => x.HasClass("timer"));
            if (nodeTimer is null) return TimeSpan.Zero;
            var strSec = new string(nodeTimer.GetAttributeValue("value", "0").Where(c => char.IsNumber(c)).ToArray());
            if (string.IsNullOrEmpty(strSec)) return TimeSpan.Zero;
            int sec = int.Parse(strSec);
            return TimeSpan.FromSeconds(sec);
        }

        public HtmlNode GetFinishButton(HtmlDocument doc)
        {
            var finishClass = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishClass is null) return null;
            return finishClass.Descendants("button").FirstOrDefault();
        }

        public HtmlNode GetConfirmFinishNowButton(HtmlDocument doc)
        {
            var dialog = doc.GetElementbyId("finishNowDialog");
            if (dialog is null) return null;
            return dialog.Descendants("button").FirstOrDefault();
        }
    }
}