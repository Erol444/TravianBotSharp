using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsMovementModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Parsers
{
    public static class TroopsMovementParser
    {
        /// <summary>
        /// Parse troops from the overview tab inside the rally point
        /// </summary>
        public static List<TroopsMovementRallyPoint> ParseTroopsOverview(Account acc, HtmlDocument html)
        {
            var ret = new List<TroopsMovementRallyPoint>();

            var attacks = html.DocumentNode.Descendants("table").Where(x => x.HasClass("troop_details"));
            if (attacks == null) return ret;

            var now = TimeParser.GetServerTime(html);

            foreach (var attackNode in attacks)
            {
                var attack = new TroopsMovementRallyPoint();

                var movementClass = attackNode.GetClasses().FirstOrDefault(x => x != "troop_details");
                attack.MovementType = ParseMovementClass(movementClass);

                // If attack.Troops.Sum() is less than 11, we are able to view troop types attacking
                attack.Troops = ParseIncomingTroops(attackNode);

                var infos = attackNode.Descendants("tbody").FirstOrDefault(x => x.HasClass("infos"));

                attack.Arrival = now.Add(TimeParser.ParseTimer(infos));

                var sourceId = MapParser.GetKarteHref(attackNode.Descendants("td").First(x => x.HasClass("role")));
                attack.SourceCoordinates = MapHelper.CoordinatesFromKid(sourceId, acc);

                var targetId = MapParser.GetKarteHref(attackNode.Descendants("td").First(x => x.HasClass("troopHeadline")));
                attack.TargetCoordinates = MapHelper.CoordinatesFromKid(targetId, acc);

                var unitImg = attackNode.Descendants("img").First(x => x.HasClass("unit"));
                var unitInt = Parser.RemoveNonNumeric(unitImg.GetClasses().First(x => x != "unit"));
                int tribeInt = (int)(unitInt / 10);
                // ++ since the first element in Classificator.TribeEnum is Any, second is Romans.
                tribeInt++;
                attack.Tribe = ((Classificator.TribeEnum)tribeInt);

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

        /// <summary>
        /// Gets available troops inside the "Send Troops" tab of the rally point
        /// </summary>
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

        public static List<TroopMovementDorf1> ParseDorf1Movements(HtmlDocument html)
        {
            var ret = new List<TroopMovementDorf1>();

            var movements = html.GetElementbyId("movements");
            if (movements == null) return ret;

            foreach (var movement in movements.Descendants("tr"))
            {
                var img = movement.Descendants("img").FirstOrDefault();
                if (img == null) continue; // Not a movement row
                var movementType = ParseMovementImg(img.GetClasses().First());

                var numStr = movement.Descendants("div").First(x => x.HasClass("mov")).InnerText;
                var num = (int)Parser.RemoveNonNumeric(numStr);

                var time = TimeParser.ParseTimer(movement.Descendants("div").First(x => x.HasClass("dur_r")));

                ret.Add(new TroopMovementDorf1()
                {
                    Type = movementType,
                    Count = num,
                    Time = DateTime.Now.Add(time)
                });
            }
            return ret;
        }

        private static MovementTypeDorf1 ParseMovementImg(string imgClass)
        {
            switch (imgClass)
            {
                case "att1": return MovementTypeDorf1.IncomingAttack;
                case "att2": return MovementTypeDorf1.OutgoingAttack;
                case "att3": return MovementTypeDorf1.IncomingAttackOasis;
                case "def1": return MovementTypeDorf1.IncomingReinforcement;
                case "def2": return MovementTypeDorf1.OutgoingReinforcement;
                case "def3": return MovementTypeDorf1.IncomingReinforcementOasis;
                case "hero_on_adventure": return MovementTypeDorf1.HeroAdventure;
                case "settlersOnTheWay": return MovementTypeDorf1.Settlers;
                default: throw new Exception("Failed to parse movement image! Class: " + imgClass);
            }
        }

        /// <summary>
        /// Parse movement type from class name of the (html) table inside rally point, overview tab
        /// </summary>
        private static MovementTypeRallyPoint ParseMovementClass(string className)
        {
            if (string.IsNullOrEmpty(className)) return MovementTypeRallyPoint.atHome;
            if (Enum.TryParse(className, out MovementTypeRallyPoint type)) return type;
            return MovementTypeRallyPoint.atHome;
        }
    }
}