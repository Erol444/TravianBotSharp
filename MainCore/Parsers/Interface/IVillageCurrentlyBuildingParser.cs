using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace MainCore.Parser.Interface
{
    public interface IVillageCurrentlyBuildingParser
    {
        public List<HtmlNode> GetItems(HtmlDocument doc);

        public string GetBuildingType(HtmlNode node);

        public int GetLevel(HtmlNode node);

        public TimeSpan GetDuration(HtmlNode node);

        public HtmlNode GetFinishButton(HtmlDocument doc);

        public HtmlNode GetConfirmFinishNowButton(HtmlDocument doc);
    }
}