using HtmlAgilityPack;
using System;
using System.Linq;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Parsers
{
    public static class HeroParser
    {
        public static HeroInfo GetHeroInfo(HtmlAgilityPack.HtmlDocument htmlDoc)
        {

            var values = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value == "element current powervalue"));
            var attributes = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value == "element current powervalue tooltip"));
            var lvl = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value == "titleInHeader")).FirstOrDefault();
            var resSelected = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value == "resourcePick")).FirstOrDefault();
            byte resSelectedByte = 255;
            var production = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value == "production tooltip")).FirstOrDefault().ChildNodes[1].ChildNodes[3].InnerText;

            for (byte i = 0; i < 5; i++)
            {
                var selected = resSelected.ChildNodes[(i * 2) + 1].ChildNodes[1].ChildNodes[1].Attributes.Where(x => x.Value == "checked").FirstOrDefault();
                if (selected != null) resSelectedByte = i;
            }

            //TODO: check hero items inventory and items on hero

            return new HeroInfo()
            {
                Health = (int)Parser.ParseNum(values.ElementAt(0).ChildNodes[1].InnerText.Replace("%", "")),
                Experience = (int)Parser.ParseNum(values.ElementAt(1).ChildNodes[1].InnerText),
                LastChecked = DateTime.Now,
                FightingStrengthPoints = (int)Parser.ParseNum(attributes.ElementAt(0).ChildNodes[1].InnerText.Replace("%", "")),
                OffBonusPoints = (int)Parser.ParseNum(attributes.ElementAt(1).ChildNodes[1].InnerText.Replace("%", "")),
                DeffBonusPoints = (int)Parser.ParseNum(attributes.ElementAt(2).ChildNodes[1].InnerText.Replace("%", "")),
                ResourcesPoints = (int)Parser.ParseNum(attributes.ElementAt(3).ChildNodes[1].InnerText.Replace("%", "")),
                AvaliblePoints = (int)Parser.ParseNum(htmlDoc.GetElementbyId("availablePoints").InnerText.Split('/')[1]),
                Level = (int)Parser.ParseNum(lvl.InnerText.Split(' ').Last()),
                SelectedResource = resSelectedByte,
                HeroProduction = (int)Parser.ParseNum(production)
            };
        }
        public static int GetAdventureNum(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var adv44 = htmlDoc.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("adventureWhite"));
                    var bubble = adv44.Descendants().FirstOrDefault(x => x.HasClass("speechBubbleContent"));
                    if (bubble == null) return 0; //No bubble, no adventures
                    return (int)Parser.RemoveNonNumeric(bubble.InnerText);
                case Classificator.ServerVersionEnum.T4_5:
                    var adv45 = htmlDoc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("adventure"));
                    var num = adv45.ChildNodes.FirstOrDefault(x => x.HasClass("content") && x.Name == "div");
                    if (num == null) return -1;
                    return (int)Parser.RemoveNonNumeric(num.InnerText);
                default: return -1;
            }
        }
        public static bool LeveledUp(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            //<i class="levelUp"></i>
            return htmlDoc.DocumentNode.Descendants("i").FirstOrDefault(x => x.HasClass("levelUp")) != null;
        }
        public static Hero.StatusEnum HeroStatus(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var HeroStatus = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatusMessage")).ChildNodes.FirstOrDefault(x => x.Name == "img").GetAttributeValue("class", "");
                    switch (HeroStatus)
                    {
                        case "heroStatus101Regenerate":
                            return Hero.StatusEnum.Regenerating;
                        case "heroStatus101":
                            return Hero.StatusEnum.Dead;
                        case "heroStatus100":
                            return Hero.StatusEnum.Home;
                        case "heroStatus50":
                            return Hero.StatusEnum.Away;
                        default: return Hero.StatusEnum.Unknown;
                    }
                case Classificator.ServerVersionEnum.T4_5:
                    var heroStatus5 = htmlDoc.DocumentNode.Descendants("div").First(x => x.HasClass("heroStatus")).Descendants().FirstOrDefault(x=>x.Name == "svg");
                    if (heroStatus5 == null) return Hero.StatusEnum.Unknown;
                    var str = heroStatus5.GetClasses().FirstOrDefault();
                    if (str == null) return Hero.StatusEnum.Unknown;
                    switch (str)
                    {
                        case "heroRunning":
                            return Hero.StatusEnum.Away;
                        case "heroHome":
                            return Hero.StatusEnum.Home;
                        case "heroDead":
                            return Hero.StatusEnum.Dead;
                        case "heroReviving":
                            return Hero.StatusEnum.Regenerating;
                        default: return Hero.StatusEnum.Unknown;
                            //TODO ADD FOR DEAD, REGENERATING
                    }

            }
            return Hero.StatusEnum.Unknown;
        }
        public static int GetHeroHealth(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var health = htmlDoc.GetElementbyId("sidebarBoxHero").Descendants("div").FirstOrDefault(x => x.HasClass("bar")).Attributes.FirstOrDefault(x => x.Name == "style").Value.Split(':')[1].Replace("%", "");
                    health = health.Split('.')[0];
                    return (int)Parser.RemoveNonNumeric(health);
                case Classificator.ServerVersionEnum.T4_5:
                    var path = htmlDoc.GetElementbyId("healthMask").Descendants("path").FirstOrDefault();
                    if (path == null) return 0;
                    var commands = path.GetAttributeValue("d", "").Split(' ');
                    var xx = double.Parse(commands[commands.Length - 2]);
                    var yy = double.Parse(commands[commands.Length - 1]);

                    var rad = Math.Atan2(yy - 55, xx - 55);
                    return (int)Math.Round(-56.173 * rad + 96.077);
            }
            return 0;
        }
        public static bool IsLeveledUp(HtmlDocument htmlDoc)
        {
            //htmlDoc.GetElementbyId("sidebarBoxHero").Descendants("div").FirstOrDefault(x=>x.HasClass("sidebarBoxInnerBox")).ChildNodes.FirstOrDefault(x=>x.Name == "div").
            return false;
        }

        public static int? GetHeroVillageId(HtmlDocument htmlDoc)
        {
            var statusMessage = htmlDoc.DocumentNode.Descendants("div").LastOrDefault(x => x.HasClass("heroStatusMessage"));

            var href = statusMessage.ChildNodes.FirstOrDefault(x => x.Name == "a")?.GetAttributeValue("href", "");

            if (href == null) return null;

            return Convert.ToInt32(href.Split('=')[1]);
        }

        public static int GetAvailablePoints(HtmlDocument htmlDoc)
        {
            var span = htmlDoc.GetElementbyId("availablePoints");
            var points = (int)Parser.RemoveNonNumeric(span.InnerText);
            return points;
        }
    }
}
