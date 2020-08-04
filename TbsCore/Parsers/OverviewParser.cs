using HtmlAgilityPack;
using System;
using System.Linq;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Parsers
{
    /// <summary>
    /// To parse village overviews (if player has plus account).
    /// </summary>
    public static class OverviewParser
    {
        public static void UpdateTroopsLevels(HtmlAgilityPack.HtmlDocument htmlDoc, ref Account acc)
        {
            var table = htmlDoc.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));

            // Go through all villages
            foreach (var villTr in table.ChildNodes.First(x => x.Name == "tbody").ChildNodes.Where(x => x.Name == "tr").ToList())
            {
                var villId = Convert.ToInt32(villTr.Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[1].Split('&')[0]);
                var vill = acc.Villages.FirstOrDefault(x => x.Id == villId);

                var tds = villTr.ChildNodes.Where(x => x.Name == "td").ToArray();
                // Go through each troops
                for (int i = 2; i < tds.Count(); i++)
                {
                    var level = (int)Parser.RemoveNonNumeric(tds[i].InnerText);
                    var troop = i - 1 + 10 * ((int)acc.AccInfo.Tribe - 1);
                    var troopEnum = (Classificator.TroopsEnum)troop;

                    if (level > 0)
                    {
                        if (!vill.Troops.Researched.Any(x => x == troopEnum))
                        {
                            vill.Troops.Researched.Add(troopEnum);
                            vill.Troops.ToResearch.Remove(troopEnum);
                        }

                        if (!vill.Troops.Levels.Any(x => x.Troop == troopEnum))
                        {
                            vill.Troops.Levels.Add(new Models.TroopsModels.TroopLevel()
                            {
                                Troop = troopEnum,
                                Level = level
                            });
                        }
                    }
                }
            }
        }

        public static void UpdateExpansions(HtmlDocument html, ref Account acc)
        {
            // TODO: parse available/used expansions
        }
    }
}
