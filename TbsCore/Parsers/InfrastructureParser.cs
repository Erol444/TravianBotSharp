using System;
using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Models.VillageModels;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Parsers
{
    public static class InfrastructureParser
    {
        public static List<Building> GetBuildings(Account acc, HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var fields = htmlDoc.GetElementbyId("village_map").ChildNodes.Where(x => x.Name == "div").ToList();
            List<Building> buildings = new List<Building>();
            for (byte i = 0; i < fields.Count; i++)
            {
                var vals = fields[i].GetAttributeValue("class", "").Split(' ');

                var location = (int)Parser.RemoveNonNumeric(vals.FirstOrDefault(x => x.StartsWith("a")));
                if (location <= 18 || location > 40) continue;

                var gid = Convert.ToByte(vals.FirstOrDefault(x => x.StartsWith("g")).Replace("g", ""));

                byte lvl;
                var lvlNode = fields[i].Descendants("div").FirstOrDefault(x => x.HasClass("labelLayer"));
                if (lvlNode == null) lvl = 0;
                else lvl = Convert.ToByte(lvlNode.InnerText);

                var uc = fields[i].Descendants("div").FirstOrDefault(x => x.HasClass("underConstruction")) != null;
                //var b = fields[i].Child
                var building = new Building();
                buildings.Add(building.Init(
                    location,
                    lvl,//Convert.ToByte(vals[4].Replace("level", "")),
                    gid,
                    uc
                ));
            }
            buildings.FirstOrDefault(x => x.Id == 39).Type = Helpers.Classificator.BuildingEnum.RallyPoint;
            buildings.FirstOrDefault(x => x.Id == 40).Type = InfrastructureHelper.GetTribesWall(acc.AccInfo.Tribe);
            return buildings;
        }


        public static List<BuildingCurrently> CurrentlyBuilding(HtmlAgilityPack.HtmlDocument htmlDoc, Account acc)
        {
            var finishButton = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishButton == null) return null;
            var ret = new List<BuildingCurrently>();
            foreach (var row in finishButton.ParentNode.Descendants("li").ToList())
            {
                var duration = TimeParser.ParseTimer(row);
                var nameArr = row.Descendants("div").FirstOrDefault(x => x.HasClass("name")).InnerText.Split('\t'); //[1].Trim();

                var levelStr = row.Descendants("span").FirstOrDefault(x => x.HasClass("lvl")).InnerText;
                string name = nameArr.FirstOrDefault(x => !string.IsNullOrEmpty(x.Replace("\r", "").Replace("\n", "")));
                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.T4_5:

                        break;
                    case Classificator.ServerVersionEnum.T4_4:
                        name = name.Replace(levelStr, "");
                        break;

                }
                var lvl = Parser.RemoveNonNumeric(levelStr);

                ret.Add(new BuildingCurrently()
                {
                    Duration = DateTime.Now.Add(duration),
                    Level = (byte)lvl,
                    Building = Localizations.BuildingFromString(name, acc)
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
