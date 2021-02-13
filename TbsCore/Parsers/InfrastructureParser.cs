using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Parsers
{
    public static class InfrastructureParser
    {
        public static List<Building> GetBuildings(Account acc, HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            List<Building> buildings = new List<Building>();
            var villMap = htmlDoc.GetElementbyId("village_map");
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
        public static List<BuildingCurrently> CurrentlyBuilding(HtmlAgilityPack.HtmlDocument htmlDoc, Account acc)
        {
            var finishButton = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishButton == null) return null;

            var ret = new List<BuildingCurrently>();
            foreach (var row in finishButton.ParentNode.Descendants("li").ToList())
            {
                var duration = TimeParser.ParseTimer(row);
                var level = row.Descendants("span").FirstOrDefault(x => x.HasClass("lvl")).InnerText;

                ret.Add(new BuildingCurrently()
                {
                    Duration = DateTime.Now.Add(duration),
                    Level = (int)Parser.RemoveNonNumeric(level),
                });
            }
            return ret;
        }

        public static TimeSpan GetBuildDuration(HtmlAgilityPack.HtmlNode node, Classificator.ServerVersionEnum version)
        {
            var duration = node.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            if (duration != null)
            {
                switch (version)
                {
                    case Classificator.ServerVersionEnum.T4_4:
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

        public static (Classificator.BuildingEnum, int) UpgradeBuildingGetInfo(HtmlAgilityPack.HtmlNode node)
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
    }
}
