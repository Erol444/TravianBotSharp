using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.Parsers
{
    public static class ResourceParser
    {
        public static StoredResources GetResources(HtmlDocument htmlDoc)
        {
            return new StoredResources()
            {
                Resources = new Resources()
                {
                    Wood = ParseHtml(htmlDoc.GetElementbyId("l1").InnerText),
                    Clay = ParseHtml(htmlDoc.GetElementbyId("l2").InnerText),
                    Iron = ParseHtml(htmlDoc.GetElementbyId("l3").InnerText),
                    Crop = ParseHtml(htmlDoc.GetElementbyId("l4").InnerText)
                },
                LastRefresh = DateTime.Now
            };
        }
        public static Resources GetProduction(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var Res = htmlDoc.GetElementbyId("production").ChildNodes[3].ChildNodes;
            //1,3,5,7

            return new Resources()
            {
                Wood = ParseHtml(Res[1].ChildNodes[5].InnerText),
                Clay = ParseHtml(Res[3].ChildNodes[5].InnerText),
                Iron = ParseHtml(Res[5].ChildNodes[5].InnerText),
                Crop = ParseHtml(Res[7].ChildNodes[5].InnerText)
            };
        }
        private static long ParseHtml(string text)
        {
            string decoded = System.Net.WebUtility.HtmlDecode(text);
            return Parser.RemoveNonNumeric(decoded);
        }
        public static ResourceCapacity GetResourceCapacity(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            string WarehouseCap = "";
            string GranaryCap = "";

            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    WarehouseCap = htmlDoc.GetElementbyId("stockBarWarehouse").InnerText;
                    GranaryCap = htmlDoc.GetElementbyId("stockBarGranary").InnerText;
                    break;
                case Classificator.ServerVersionEnum.T4_5:
                    var cap = htmlDoc.DocumentNode.Descendants("div").Where(x => x.HasClass("capacity")).ToList();

                    WarehouseCap = cap[0].Descendants("div").FirstOrDefault(x => x.HasClass("value")).InnerText;
                    GranaryCap = cap[1].Descendants("div").FirstOrDefault(x => x.HasClass("value")).InnerText;
                    break;

            }
            return new ResourceCapacity()
            {
                WarehouseCapacity = Parser.RemoveNonNumeric(WarehouseCap),
                GranaryCapacity = Parser.RemoveNonNumeric(GranaryCap)
            };
        }
        public static List<Building> GetResourcefields(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
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
                    var fields5 = htmlDoc.GetElementbyId("resourceFieldContainer").ChildNodes.Where(x => x.Name == "div").ToList();
                    List<Building> resFields5 = new List<Building>();
                    for (int i = 0; i < 18; i++)
                    {
                        var vals = fields5.ElementAt(i).GetClasses(); //.GetAttributeValue("class", "").Split(' ');
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
            var imgs = node.Descendants("img").ToList();
            Resources res = new Resources()
            {
                //Wood = imgs.First(x => x.HasClass("r1"));
            };
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
