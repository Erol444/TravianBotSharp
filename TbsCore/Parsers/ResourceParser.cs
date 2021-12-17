using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;

namespace TbsCore.Parsers
{
    public static class ResourceParser
    {
        private static long ParseHtml(string text)
        {
            string decoded = System.Net.WebUtility.HtmlDecode(text);
            return Parser.RemoveNonNumeric(decoded);
        }

        public static List<Building> GetResourcefields(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    var fields = htmlDoc.GetElementbyId("village_map").Descendants("div").Where(x => !x.HasClass("labelLayer")).ToList();
                    List<Building> resFields = new List<Building>();
                    for (int i = 0; i < 18; i++)
                    {
                        var vals = fields.ElementAt(i).GetAttributeValue("class", "").Split(' ');
                        var building = new Building();

                        var aid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("aid")).Replace("aid", ""));
                        var lvl = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("level") && x != "level").Replace("level", ""));
                        var gid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("gid")).Replace("gid", ""));
                        var uc = vals.Contains("underConstruction");

                        building.Init(aid, lvl, gid, uc);
                        resFields.Add(building);
                    }
                    return resFields;

                case Classificator.ServerVersionEnum.T4_5:
                    var fields5 = htmlDoc.GetElementbyId("resourceFieldContainer").ChildNodes.Where(x => x.HasClass("level")).ToList();
                    List<Building> resFields5 = new List<Building>();
                    foreach (var field in fields5)
                    {
                        var vals = field.GetClasses(); //.GetAttributeValue("class", "").Split(' ');
                        //fields5.ElementAt(1).GetClasses().
                        var building = new Building();

                        var aid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("buildingSlot")).Replace("buildingSlot", ""));
                        var lvl = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("level") && x != "level").Replace("level", ""));
                        var gid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("gid")).Replace("gid", ""));
                        var uc = vals.Contains("underConstruction");

                        building.Init(aid, lvl, gid, uc);
                        resFields5.Add(building);
                    }
                    return resFields5;
            }
            return null;
        }

        public static Resources GetResourceCost(HtmlNode node)
        {
            var res = new Resources();
            if (node == null) return res;
            var resNodes = node.ChildNodes.Where(x => x.HasClass("resource") || x.HasClass("resources")).ToList();
            if (resNodes.Count < 4) return null;
            res.Wood = Parser.RemoveNonNumeric(resNodes[0].InnerText);
            res.Clay = Parser.RemoveNonNumeric(resNodes[1].InnerText);
            res.Iron = Parser.RemoveNonNumeric(resNodes[2].InnerText);
            res.Crop = Parser.RemoveNonNumeric(resNodes[3].InnerText);
            return res;
        }

        /// <summary>
        /// TODO: finish
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Resources ParseResourcesMerchants(HtmlNode node)
        {
            var td = node.Descendants("tr").First(x => x.HasClass("res")).Descendants("td").First();
            var text = td.InnerText;
            var split = text.Split(' ');
            Resources res = new Resources()
            {
                Wood = Parser.RemoveNonNumeric(split[split.Length - 4]),
                Clay = Parser.RemoveNonNumeric(split[split.Length - 3]),
                Iron = Parser.RemoveNonNumeric(split[split.Length - 2]),
                Crop = Parser.RemoveNonNumeric(split[split.Length - 1]),
            };
            return res;
        }
    }
}