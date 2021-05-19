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

            var soonest = DateTime.Now.Add(TimeParser.ParseTimer(table));

            return soonest;
        }

        public static List<MerchantsUnderWay> ParseTransits(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var formulat = htmlDoc.GetElementbyId("merchantsOnTheWay");

            var underWay = new List<MerchantsUnderWay>();
            TransitType transitType = default;
            foreach (var child in formulat.ChildNodes)
            {
                if (child.HasClass("spacer"))
                {
                    transitType = Localizations.MercahntDirectionFromString(child.InnerText);
                    continue;
                }
                else if (child.HasClass("traders"))
                {
                    underWay.Add(new MerchantsUnderWay()
                    {
                        Arrival = DateTime.Now.Add(TimeParser.ParseTimer(child)),
                        TargetVillageId = (int)Parser.RemoveNonNumeric(child.Descendants("td").First(x => x.HasClass("dorf")).Descendants("a").First().GetAttributeValue("href", "")),
                        RepeatTimes = (int)Parser.RemoveNonNumeric(child.Descendants("div").First(x => x.HasClass("repeat")).InnerText),
                        Transit = transitType,
                        Resources = ResourceParser.ParseResourcesMerchants(child),
                    });
                }
            }
            return underWay;
        }
    }
}