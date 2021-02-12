
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Parsers
{
    public static class TroopsMovementParser
    {
        public static List<TroopsMovementModel> ParseIncomingAttacks(Account acc, HtmlDocument html)
        {
            var ret = new List<TroopsMovementModel>();

            var attacks = html.DocumentNode.Descendants("table").Where(x => x.HasClass("troop_details"));
            if (attacks == null) return ret;

            foreach (var attackNode in attacks)
            {
                var attack = new TroopsMovementModel();

                attack.MovementType = MovementType.Attack;
                if (attackNode.HasClass("inRaid")) attack.MovementType = MovementType.Raid;

                // If attack.Troops.Sum() is less than 11, we are able to view troop types attacking
                attack.Troops = ParseIncomingTroops(attackNode);

                var infos = attackNode.Descendants("tbody").FirstOrDefault(x => x.HasClass("infos"));
                attack.Arrival = DateTime.Now.Add(TimeParser.ParseTimer(infos));

                var kid = MapParser.GetKarteHref(attackNode.Descendants("td").First(x => x.HasClass("role")));
                attack.Coordinates = MapHelper.CoordinatesFromKid(kid ?? 0, acc);

                ret.Add(attack);
            }
            return ret;
        }

        /// <summary>
        /// If account has spies art or attacking troops count is lower than rally point level,
        /// bot can see "?" on only troop types that are incoming and "0" at troop types that 
        /// are not present in attack
        /// </summary>
        private static int[] ParseIncomingTroops(HtmlNode attackNode) 
        {
            var troopsBody = attackNode.Descendants("tbody").First(x => x.HasClass("last") && x.HasClass("units"));

            var troops = troopsBody.Descendants("td").Where(x => x.HasClass("unit")).ToList();
            int[] ret = new int[troops.Count];
            for (int i = 0; i < troops.Count; i++)
            {
                ret[i] = troops[i].InnerHtml.Trim() == "?" ? 1 : 0;
            }
            return ret;
        }

        /// <summary>
        /// Get page count from paginator
        /// </summary>
        /// <param name="html"></param>
        /// <returns>Page count</returns>
        public static int GetPageCount(HtmlDocument html)
        {
            var build = html.GetElementbyId("build");

            var spans = build.Descendants("div")
                .First(x => x.HasClass("paginator"))
                .ChildNodes
                .Where(x => x.HasClass("number"));

            return spans.Count();
        }

        public static TimeSpan GetTimeOfMovement(HtmlDocument html)
        {
            var content = html.GetElementbyId("content");
            var div = content.Descendants("div").FirstOrDefault(x => x.HasClass("in"));
            return TimeParser.ParseDuration(div.InnerText);
        }

        public static DateTime GetArrivalTime(HtmlDocument html)
        {
            var content = html.GetElementbyId("content");
            var div = content.Descendants("div").FirstOrDefault(x => x.HasClass("at"));
            var timer = div.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));

            var dur = TimeParser.ParseDuration(timer.InnerText);
            return DateTime.Today.Add(dur);
        }

        internal static int[] GetTroopsInRallyPoint(HtmlDocument html)
        {
            int[] ret = new int[11];
            // No rally point!
            if (html.GetElementbyId("contract_building16") != null) return ret;

            var tds = html.GetElementbyId("troops").Descendants("td");
            foreach (var td in tds)
            {
                var troop = td.Descendants("input").FirstOrDefault(x => x.HasClass("text"))?.GetAttributeValue("name", "");
                if (string.IsNullOrEmpty(troop)) continue; // For spaceholder between last horse and hero

                var troopNum = (int)Parser.RemoveNonNumeric(troop);

                var troopCount = td.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("href", "") == "#")?.InnerText;
                var troopCountInt = troopCount == null ? 0 : (int)Parser.RemoveNonNumeric(WebUtility.HtmlDecode(troopCount));
                ret[troopNum - 1] = troopCountInt;
            }
            return ret;
        }
    }
}
