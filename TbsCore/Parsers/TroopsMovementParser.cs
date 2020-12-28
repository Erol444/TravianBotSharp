
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.AttackModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Parsers
{
    public static class TroopsMovementParser
    {
        public static List<IncomingAttackModel> FullAttackWithHero(HtmlDocument html)
        {
            var ret = new List<IncomingAttackModel>();
            var attacks = html.DocumentNode.Descendants("table").Where(x => x.HasClass("troop_details"));
            if (attacks == null) return ret;
            var count = attacks.Count();
            foreach (var attack in attacks)
            {
                var attackModel = new IncomingAttackModel();

                attackModel.MovementType = MovementType.Attack;
                if (attack.HasClass("inRaid")) attackModel.MovementType = MovementType.Raid;

                var troops = attack.Descendants("tbody").FirstOrDefault(x => x.HasClass("last") && x.HasClass("units"));
                var troopsNum = troops.Descendants("td").Where(x => x.HasClass("unit"));
                // Hero can be "0" -> false only if user has stronger spies art or number of attacking troops
                // are below rally point level
                attackModel.Hero = false;
                if (troopsNum.Last().InnerText == "?") attackModel.Hero = true;

                var infos = attack.Descendants("tbody").FirstOrDefault(x => x.HasClass("infos"));
                attackModel.Arrival = DateTime.Now.Add(TimeParser.ParseTimer(infos));
            }
            return ret;
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
