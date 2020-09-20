using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Models.TroopsModels;

namespace TravBotSharp.Files.Parsers
{
    public static class TroopsParser
    {
        public static List<TroopsRaw> GetTroopsInVillage(HtmlDocument htmlDoc)
        {
            var troopsNode = htmlDoc.GetElementbyId("troops").ChildNodes[3].ChildNodes.Where(x => x.Name == "tr");
            List<TroopsRaw> troopsList = new List<TroopsRaw>();
            foreach (var uniqueTroop in troopsNode)
            {
                var troop = new TroopsRaw();
                troop.Number = int.Parse(uniqueTroop.ChildNodes[3].InnerHtml.Replace(",", ""));
                var unit = uniqueTroop.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes.FirstOrDefault(x => x.Name == "class").Value.Split(' ')[1].Replace("u", "");

                if (byte.TryParse(unit, out byte result))
                {
                    troop.Type = result; //any other troop that is not hero
                }
                else troop.Type = 0; //Hero
                troopsList.Add(troop);
            }
            return troopsList;
        }

        //  return when research will be finished and how much it costs
        public static (TimeSpan, Resources) AcademyResearchCost(HtmlDocument htmlDoc, Classificator.TroopsEnum troop)
        {
            var troopNode = htmlDoc.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            if (troopNode == null) return (new TimeSpan(), null);
            while (!troopNode.HasClass("information")) troopNode = troopNode.ParentNode;

            var duration = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            TimeSpan dur = TimeParser.ParseDuration(duration);

            var resWrapper = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var res = ResourceParser.GetResourceCost(resWrapper);

            return (dur, res);
        }

        //  Return how long troop trains (in ms) and how much it costs
        public static (TimeSpan, Resources) GetTrainCost(HtmlDocument htmlDoc, Classificator.TroopsEnum troop)
        {
            var troopNode = htmlDoc.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int)troop));
            if (troopNode == null) return (new TimeSpan(), null);
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            var duration = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            TimeSpan dur = TimeParser.ParseDuration(duration);

            var resWrapper = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            Resources res = ResourceParser.GetResourceCost(resWrapper);

            return (dur, res);
        }

        public static List<TroopsCurrentlyTraining> GetTroopsCurrentlyTraining(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var list = new List<TroopsCurrentlyTraining>();
            var table = htmlDoc.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            if (table == null) return list;
            var Tbody = table.ChildNodes.FirstOrDefault(x => x.Name == "tbody");
            var troop = Tbody.Descendants("img").Where(x => x.HasClass("unit")).ToList();
            var time = Tbody.Descendants("span").Where(x => x.HasClass("timer")).ToList();

            for (int i = 0; i < troop.Count; i++)
            {
                var ct = new TroopsCurrentlyTraining();
                var cls = troop[i].GetClasses().FirstOrDefault(x => x != "unit");
                ct.Troop = (Classificator.TroopsEnum)Parser.RemoveNonNumeric(cls);
                ct.TrainNumber = (int)Parser.RemoveNonNumeric(troop[i].NextSibling.InnerText);
                ct.FinishTraining = DateTime.Now.AddSeconds((int)time[i].GetAttributeValue("value", 0));
                list.Add(ct);
            }
            return list;
        }

        /// <summary>
        /// For smithy usage
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<TroopLevel> GetTroopLevels(HtmlDocument html)
        {
            var list = new List<TroopLevel>();
            //Get all researched units
            var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));
            if (researches == null || researches.Count() == 0) return null;
            foreach (var research in researches)
            {
                var lvl = research.Descendants("span").First(x => x.HasClass("level")).InnerText;
                if (lvl.Contains("+")) //troop is currently being improved. Only get current level
                {
                    lvl = lvl.Split('+')[0];
                }
                var troopLvl = new TroopLevel()
                {
                    Troop = GetTroopFromImage(research),
                    Level = (int)Parser.RemoveNonNumeric(lvl),
                    UpgradeCost = ResourceParser.GetResourceCost(research.Descendants("div").First(x => x.HasClass("showCosts"))),
                    TimeCost = TimeParser.ParseDuration(research), //TODO!
                };
                list.Add(troopLvl);
            }
            return list;
        }

        /// <summary>
        /// Method to get currently improving troops inside the smithy.
        /// </summary>
        /// <param name="html">HtmlDocument of the page</param>
        /// <returns></returns>
        public static List<TroopCurrentlyImproving> GetImprovingTroops(HtmlDocument html)
        {
            var list = new List<TroopCurrentlyImproving>();
            var table = html.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            if (table == null) return list;
            //If we have plus acc, we can have 2 improvements simultaneously
            var rows = table.ChildNodes.First(x => x.Name == "tbody").ChildNodes.Where(x => x.Name == "tr");
            foreach (var row in rows)
            {
                var desc = row.Descendants("td").First(x => x.HasClass("desc"));
                (Classificator.TroopsEnum, int) troop = GetTroopAndLevel(desc);
                var time = TimeParser.ParseTimer(row);
                list.Add(new TroopCurrentlyImproving
                {
                    Level = troop.Item2,
                    Troop = troop.Item1,
                    Time = time
                });
            }
            return list;
        }

        /// <summary>
        /// For getting smithy upgrades
        /// </summary>
        /// <param name="node">HtmlNode, no need for it to be direct parent</param>
        /// <returns></returns>
        public static (Classificator.TroopsEnum, int) GetTroopAndLevel(HtmlNode node)
        {
            var troop = GetTroopFromImage(node);
            var level = node.Descendants("span").First(x => x.HasClass("level")).InnerText;
            return (troop, (int)Parser.RemoveNonNumeric(level));
        }

        /// <summary>
        /// Gets troop classificator from troop image
        /// </summary>
        /// <param name="node">HtmlNode, no need for it to be direct parent</param>
        /// <returns></returns>
        public static Classificator.TroopsEnum GetTroopFromImage(HtmlNode node)
        {
            var img = node.Descendants("img").First(x => x.HasClass("unit"));
            var troopNum = Parser.RemoveNonNumeric(img.GetClasses().First(x => x != "unit"));
            return (Classificator.TroopsEnum)troopNum;
        }

        /// <summary>
        /// Parses available/present troops of this kind.
        /// </summary>
        /// <param name="node">Details node of the troop</param>
        /// <returns>Number of troops available</returns>
        public static long ParseAvailable(HtmlNode node)
        {
            var info = node.Descendants("span").FirstOrDefault(x => x.HasClass("furtherInfo"));
            if (info == null) return 0;
            return Parser.RemoveNonNumeric(info.InnerText);
        }
    }
}