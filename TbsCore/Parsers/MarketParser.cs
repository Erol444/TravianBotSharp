using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.ResourceModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Parsers
{
    public static class MarketParser
    {
        public static (int, int) GetMerchantsNumber(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var tradersNode = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("traderCount"));
            if (tradersNode == null) return (0, 0);

            var text = tradersNode.InnerText.Split('/');
            int current = (int)Parser.RemoveNonNumeric(text[0]);
            int all = (int)Parser.RemoveNonNumeric(text[1]);
            return (current, all);
        }

        /// <summary>
        /// Get soonest time own merchant arrive (ignore fact they go away or go back village)
        /// </summary>
        /// <param name="htmlDoc">Html</param>
        public static DateTime GetSoonestMerchant(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            // i know id is incoming, but trust me =))
            var divOwnMerchants = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("incomingMerchants"));

            var table = divOwnMerchants.Descendants("table").FirstOrDefault();

            var now = TimeParser.GetServerTime(htmlDoc);

            var soonest = now.Add(TimeParser.ParseTimer(table));

            return soonest;
        }
    }
}