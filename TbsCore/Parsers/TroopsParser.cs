using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Parsers
{
    public static class TroopsParser
    {
        /// <summary>
        ///     Parses the troops inside the village from dorf1
        /// </summary>
        public static Dictionary<Classificator.TroopsEnum, int> GetTroopsInVillage(HtmlDocument htmlDoc)
        {
            var ret = new Dictionary<Classificator.TroopsEnum, int>();

            var troopsNode = htmlDoc.GetElementbyId("troops").Descendants("tbody").First().Descendants("tr");
            foreach (var row in troopsNode)
            {
                var num = row.Descendants().First(x => x.HasClass("num")).InnerHtml;
                ret.Add(GetTroopFromImage(row), (int) Parser.RemoveNonNumeric(num));
            }

            return ret;
        }

        /// <summary>
        ///     Gets the research time and resource cost for the specific troop. TODO: get this from TravianData.TroopsData
        /// </summary>
        public static (TimeSpan, Resources) AcademyResearchCost(HtmlDocument htmlDoc, Classificator.TroopsEnum troop)
        {
            var troopNode = htmlDoc.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int) troop));
            if (troopNode == null) return (new TimeSpan(), null);
            while (!troopNode.HasClass("information")) troopNode = troopNode.ParentNode;

            var duration = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            var dur = TimeParser.ParseDuration(duration);

            var resWrapper = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var res = ResourceParser.GetResourceCost(resWrapper);

            return (dur, res);
        }

        /// <summary>
        ///     Gets the training time (in ms) and resource cost for the specific troop. TODO: get this from TravianData.TroopsData
        /// </summary>
        public static (TimeSpan, Resources) GetTrainCost(HtmlDocument htmlDoc, Classificator.TroopsEnum troop)
        {
            var troopNode = htmlDoc.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("u" + (int) troop));
            if (troopNode == null) return (new TimeSpan(), null);
            while (!troopNode.HasClass("details")) troopNode = troopNode.ParentNode;

            var duration = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            var dur = TimeParser.ParseDuration(duration);

            var resWrapper = troopNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var res = ResourceParser.GetResourceCost(resWrapper);

            return (dur, res);
        }

        /// <summary>
        ///     Parse currently training troops inside barracks/stable/workshop/GB/GS
        /// </summary>
        public static List<TroopsCurrentlyTraining> GetTroopsCurrentlyTraining(HtmlDocument htmlDoc)
        {
            var list = new List<TroopsCurrentlyTraining>();
            var table = htmlDoc.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            if (table == null) return list;
            var Tbody = table.ChildNodes.FirstOrDefault(x => x.Name == "tbody");
            var troop = Tbody.Descendants("img").Where(x => x.HasClass("unit")).ToList();
            var time = Tbody.Descendants("span").Where(x => x.HasClass("timer")).ToList();

            for (var i = 0; i < troop.Count; i++)
            {
                var ct = new TroopsCurrentlyTraining();
                var cls = troop[i].GetClasses().FirstOrDefault(x => x != "unit");
                ct.Troop = (Classificator.TroopsEnum) Parser.RemoveNonNumeric(cls);
                ct.TrainNumber = (int) Parser.RemoveNonNumeric(troop[i].NextSibling.InnerText);
                ct.FinishTraining = DateTime.Now.AddSeconds(time[i].GetAttributeValue("value", 0));
                list.Add(ct);
            }

            return list;
        }

        /// <summary>
        ///     For smithy usage
        /// </summary>
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
                    lvl = lvl.Split('+')[0];
                var troop = new TroopLevel();
                troop.Troop = GetTroopFromImage(research);
                troop.Level = (int) Parser.RemoveNonNumeric(lvl);

                if (troop.Level != 20) // If max level, there is no upgrade/time cost
                {
                    troop.UpgradeCost = ResourceParser.GetResourceCost(research
                        .Descendants("div")
                        .FirstOrDefault(x => x.HasClass("showCosts") || x.HasClass("resourceWrapper")));
                    troop.TimeCost = TimeParser.ParseDuration(research); //TODO!
                }

                list.Add(troop);
            }

            return list;
        }

        /// <summary>
        ///     Method to get currently improving troops inside the smithy.
        /// </summary>
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
                var troop = GetTroopAndLevel(desc);
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
        ///     For getting smithy upgrades
        /// </summary>
        /// <param name="node">HtmlNode, not needed to be direct parent</param>
        /// <returns></returns>
        public static (Classificator.TroopsEnum, int) GetTroopAndLevel(HtmlNode node)
        {
            var troop = GetTroopFromImage(node);
            var level = node.Descendants("span").First(x => x.HasClass("level")).InnerText;
            return (troop, (int) Parser.RemoveNonNumeric(level));
        }

        /// <summary>
        ///     Gets troop from troop image
        /// </summary>
        /// <param name="node">HtmlNode, no needed to be direct parent</param>
        public static Classificator.TroopsEnum GetTroopFromImage(HtmlNode node)
        {
            var img = node.Descendants("img").First(x => x.HasClass("unit"));
            var troopNum = Parser.RemoveNonNumeric(img.GetClasses().First(x => x != "unit"));
            return (Classificator.TroopsEnum) troopNum;
        }

        /// <summary>
        ///     Parses available/present troops of this kind.
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