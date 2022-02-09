using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;
using TbsCore.TravianData;
using HtmlAgilityPack;

namespace TbsCore.Parsers
{
    public static class InfrastructureParser
    {
        public static List<Building> GetBuildings(Account acc, HtmlDocument htmlDoc)
        {
            List<Building> buildings = new List<Building>();
            HtmlAgilityPack.HtmlNode villMap = null;
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_5:
                    villMap = htmlDoc.GetElementbyId("villageContent");
                    break;

                case Classificator.ServerVersionEnum.TTwars:
                    villMap = htmlDoc.GetElementbyId("village_map");
                    break;
            }

            if (villMap == null) return buildings;

            var fields = villMap.ChildNodes.Where(x => x.Name == "div").ToList();

            for (byte i = 0; i < fields.Count; i++)
            {
                var vals = fields[i].GetAttributeValue("class", "").Split(' ');

                var location = (int)Parser.RemoveNonNumeric(vals.FirstOrDefault(x => x.StartsWith("a")));
                if (location <= 18 || location > 40) continue;

                var gid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("g")).Replace("g", ""));

                byte lvl;
                // TODO: aid
                var lvlNode = fields[i].Descendants().FirstOrDefault(x => x.HasClass("aid" + location));
                if (lvlNode == null) lvl = 0;
                else lvl = Convert.ToByte(lvlNode.InnerText);

                var uc = fields[i].Descendants().FirstOrDefault(x => x.HasClass("underConstruction")) != null;
                //var b = fields[i].Child
                var building = new Building();
                buildings.Add(building.Init(
                    location,
                    lvl,//Convert.ToByte(vals[4].Replace("level", "")),
                    gid,
                    uc
                ));
            }
            buildings.FirstOrDefault(x => x.Id == 39).Type = Classificator.BuildingEnum.RallyPoint;
            buildings.FirstOrDefault(x => x.Id == 40).Type = BuildingsData.GetTribesWall(acc.AccInfo.Tribe);
            return buildings;
        }

        /// <summary>
        /// Get currently building (upgrading/constructing) buildings from dorf1/dorf2
        /// </summary>
        public static List<BuildingCurrently> CurrentlyBuilding(HtmlDocument htmlDoc, Account acc)
        {
            var finishButton = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishButton == null) return null;

            var ret = new List<BuildingCurrently>();
            foreach (var row in finishButton.ParentNode.Descendants("li").ToList())
            {
                var duration = TimeParser.ParseTimer(row);
                var level = row.Descendants("span").FirstOrDefault(x => x.HasClass("lvl")).InnerText;
                var strName = row.Descendants("div").FirstOrDefault(x => x.HasClass("name")).ChildNodes[0].InnerText.Trim(new[] { '\t', '\n', '\r', ' ' });
                strName = string.Join("", strName.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                Enum.TryParse(strName, out Classificator.BuildingEnum name);

                ret.Add(new BuildingCurrently()
                {
                    Building = name,
                    Duration = DateTime.Now.Add(duration),
                    Level = (byte)Parser.RemoveNonNumeric(level),
                });
            }
            return ret;
        }

        public static TimeSpan GetBuildDuration(HtmlNode node, Classificator.ServerVersionEnum version)
        {
            var duration = node.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            if (duration != null)
            {
                switch (version)
                {
                    case Classificator.ServerVersionEnum.TTwars:
                        return TimeParser.ParseDuration(duration.InnerText);

                    case Classificator.ServerVersionEnum.T4_5:
                        var dur = duration.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
                        if (dur != null)
                        {
                            return TimeParser.ParseDuration(dur.InnerText);
                        }
                        break;
                }
            }
            return new TimeSpan();
        }

        public static (Classificator.BuildingEnum, int) UpgradeBuildingGetInfo(HtmlNode node)
        {
            var classes = node.GetClasses().ToList();

            int level = -1;
            var building = Classificator.BuildingEnum.Site;

            var lvlClass = classes.FirstOrDefault(x => x.StartsWith("level"));
            if (lvlClass != null)
            {
                level = int.Parse(lvlClass.Replace("level", ""));
            }

            var buildingClass = classes.FirstOrDefault(x => x.StartsWith("gid"));
            if (buildingClass != null)
            {
                var num = (int)Parser.RemoveNonNumeric(buildingClass.Replace("gid", ""));
                building = (Classificator.BuildingEnum)num;
            }

            return (building, level);
        }

        public static int CurrentlyActiveTab(HtmlDocument html)
        {
            // TODO: fix for ttwars (class="container active")
            var tabs = html.DocumentNode.Descendants("a").Where(x => x.HasClass("tabItem")).ToList();
            for (int i = 0; i < tabs.Count(); i++)
            {
                if (tabs[i].HasClass("active")) return i;
            }
            return -1;
        }
    }
}