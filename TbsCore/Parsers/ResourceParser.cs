using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Parsers
{
    public static class ResourceParser
    {
        private static long ParseHtml(string text)
        {
            var decoded = WebUtility.HtmlDecode(text);
            return Parser.RemoveNonNumeric(decoded);
        }

        public static List<Building> GetResourcefields(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var fields = htmlDoc.GetElementbyId("village_map").Descendants("div")
                        .Where(x => !x.HasClass("labelLayer")).ToList();
                    var resFields = new List<Building>();
                    for (var i = 0; i < 18; i++)
                    {
                        var vals = fields.ElementAt(i).GetAttributeValue("class", "").Split(' ');
                        var building = new Building();

                        var aid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("aid")).Replace("aid", ""));
                        var lvl = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("level") && x != "level")
                            .Replace("level", ""));
                        var gid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("gid")).Replace("gid", ""));
                        var uc = vals.Contains("underConstruction");

                        building.Init(aid, lvl, gid, uc);
                        resFields.Add(building);
                    }

                    return resFields;
                case Classificator.ServerVersionEnum.T4_5:
                    var fields5 = htmlDoc.GetElementbyId("resourceFieldContainer").ChildNodes
                        .Where(x => x.HasClass("level")).ToList();
                    var resFields5 = new List<Building>();
                    foreach (var field in fields5)
                    {
                        var vals = field.GetClasses(); //.GetAttributeValue("class", "").Split(' ');
                        //fields5.ElementAt(1).GetClasses().
                        var building = new Building();

                        var aid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("buildingSlot"))
                            .Replace("buildingSlot", ""));
                        var lvl = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("level") && x != "level")
                            .Replace("level", ""));
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
        ///     TODO: finish
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Resources ParseResourcesMerchants(HtmlNode node)
        {
            var imgs = node.Descendants("img").ToList();
            var res = new Resources();
            /*
                         <tr class="res">
                <th>Resources</th>
                <td colspan="1">
                    <span class="">
                        <div class="repeat">‎‭‭2×‬‎</div>
                        <img class="r1" src="img/x.gif" alt="Lumber"> 640000
                        <img class="r2" src="img/x.gif" alt="Clay"> 920000
                        <img class="r3" src="img/x.gif" alt="Iron"> 330000
                        <img class="r4" src="img/x.gif" alt="Crop"> 0
                    </span>
                </td>
            </tr>
             */
            return res;
        }
    }
}