using HtmlAgilityPack;
using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface ITrainTroopParser
    {
        public List<HtmlNode> GetTroopNodes(HtmlDocument doc);

        public int GetTroopType(HtmlNode node);

        public Resources GetTrainCost(HtmlNode node);

        public TimeSpan GetTrainTime(HtmlNode node);

        public HtmlNode GetInputBox(HtmlNode node);

        public int GetMaxAmount(HtmlNode node);

        public HtmlNode GetTrainButton(HtmlDocument doc);

        public TimeSpan GetQueueTrainTime(HtmlDocument doc);
    }
}