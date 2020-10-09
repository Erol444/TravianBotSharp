using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Parsers
{
    public static class HeroParser
    {
        private static readonly string[] domId = new string[] {
            "attributepower",
            "attributeoffBonus",
            "attributedefBonus",
            "attributeproductionPoints"
        };

        public static bool AttributesHidden(HtmlDocument htmlDoc)
        {
            var attributes = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroPropertiesContent"));
            if (attributes?.GetClasses()?.FirstOrDefault(x => x == "hide") == null) return false;
            else return true;
        }
        public static HeroInfo GetHeroInfo(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var content = htmlDoc.GetElementbyId("content");
            var health = content.Descendants("tr")
                .FirstOrDefault(x => x.HasClass("health"))
                .Descendants("span")
                .FirstOrDefault(x => x.HasClass("value"))
                .InnerText;

            var experience = content.Descendants("tr")
                 .FirstOrDefault(x => x.HasClass("experience"))
                 .Descendants("span")
                 .FirstOrDefault(x => x.HasClass("value"))
                 .InnerText;

            string[] heroPoints = new string[4];
            for (int i = 0; i < 4; i++)
            {
                heroPoints[i] = htmlDoc.GetElementbyId(domId[i])
                    .ChildNodes
                    .FirstOrDefault(x => x.HasClass("points"))
                    .InnerText
                    .Replace("%", "");
            }

            var availablePoints = System.Net.WebUtility.HtmlDecode(htmlDoc.GetElementbyId("availablePoints").InnerText);

            var heroLevel = htmlDoc.DocumentNode.Descendants()
                .FirstOrDefault(x => x.HasClass("titleInHeader"))
                .InnerText
                .Split(' ')
                .Last();

            var production = htmlDoc.DocumentNode.Descendants()
                .FirstOrDefault(x => x.HasClass("production"))
                .Descendants("span")
                .FirstOrDefault(x => x.HasClass("value"))
                .InnerText;

            var resRadioChecked = htmlDoc.DocumentNode.Descendants("input").FirstOrDefault(x =>
                x.HasClass("radio") &&
                x.GetAttributeValue("checked", "") == "checked"
            );
            byte resSelectedByte = 0;
            if (resRadioChecked != null)
            {
                resSelectedByte = (byte) Parser.RemoveNonNumeric(resRadioChecked.GetAttributeValue("value", "0"));
            }

            var heroInfo = new HeroInfo();
            heroInfo.Health = (int)Parser.ParseNum(health.Replace("%", ""));
            heroInfo.Experience = (int)Parser.ParseNum(experience);
            heroInfo.AvaliblePoints = (int)Parser.ParseNum(availablePoints.Split('/').LastOrDefault());

            if(heroInfo.AvaliblePoints == 0)
            {
                heroInfo.FightingStrengthPoints = (int)Parser.ParseNum(heroPoints[0]);
                heroInfo.OffBonusPoints = (int)Parser.ParseNum(heroPoints[1]);
                heroInfo.DeffBonusPoints = (int)Parser.ParseNum(heroPoints[2]);
                heroInfo.ResourcesPoints = (int)Parser.ParseNum(heroPoints[3]);
            }

            heroInfo.Level = (int)Parser.ParseNum(heroLevel);
            heroInfo.SelectedResource = resSelectedByte;
            heroInfo.HeroProduction = (int)Parser.RemoveNonNumeric(production);

            return heroInfo;
        }
        /// <summary>
        /// Parses when the hero arrival will be (parsed from /hero.php)
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>TimeSpan after how much time hero arrival will happen</returns>
        public static TimeSpan GetHeroArrivalInfo(HtmlDocument html)
        {
            var statusMsg = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatusMessage"));
            if (statusMsg == null) return TimeSpan.Zero;

            return TimeParser.ParseTimer(statusMsg);
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
                    if (num == null) return 0;
                    return (int)Parser.RemoveNonNumeric(num.InnerText);
                default: return 0;
            }
        }
        public static bool LeveledUp(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    return htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("levelUp")) != null;
                case Classificator.ServerVersionEnum.T4_5:
                    return htmlDoc.DocumentNode.Descendants("i").FirstOrDefault(x => x.HasClass("levelUp")) != null;
            }
            return false;
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
                    var heroStatus5 = htmlDoc.DocumentNode.Descendants("div").First(x => x.HasClass("heroStatus")).Descendants().FirstOrDefault(x => x.Name == "svg");
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

        /// <summary>
        /// Parses the her home village href
        /// </summary>
        /// <param name="htmlDoc">Html</param>
        /// <returns>Hero's home village href id</returns>
        public static int? GetHeroVillageHref(HtmlDocument htmlDoc)
        {
            var node = htmlDoc.GetElementbyId("attributes");
            if (node == null) node = htmlDoc.GetElementbyId("content");
            if (node == null) return null;

            var href = node.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("href", "").StartsWith("/karte"));
            if (href == null) return null;

            return Convert.ToInt32(href.GetAttributeValue("href", "").Split('=').Last());
        }

        public static int GetAvailablePoints(HtmlDocument htmlDoc)
        {
            var span = htmlDoc.GetElementbyId("availablePoints");
            var points = (int)Parser.RemoveNonNumeric(span.InnerText);
            return points;
        }

        public static TimeSpan GetHeroArrival(HtmlDocument htmlDoc)
        {
            return TimeParser.ParseTimer(htmlDoc.GetElementbyId("tileDetails"));
        }

        public static List<HeroItem> GetHeroItems(HtmlDocument html)
        {
            List<HeroItem> heroItems = new List<HeroItem>();
            var inventory = html.GetElementbyId("itemsToSale");

            foreach (var itemSlot in inventory.ChildNodes)
            {
                var item = itemSlot.ChildNodes.FirstOrDefault(x => x.Id.StartsWith("item_"));
                if (item == null) continue;

                (var heroItemEnum, int amount) = ParseItemNode(item);
                if (heroItemEnum == null) continue;

                var heroItem = new HeroItem
                {
                    Item = heroItemEnum ?? Classificator.HeroItemEnum.Others_None_0,
                    Count = amount
                };

                heroItems.Add(heroItem);
            }
            return heroItems;
        }

        private static readonly Dictionary<Classificator.HeroItemCategory, string> HeroTypeIds = new Dictionary<Classificator.HeroItemCategory, string>()
        {
            { Classificator.HeroItemCategory.Helmet, "helmet" },
            { Classificator.HeroItemCategory.Left, "leftHand" },
            { Classificator.HeroItemCategory.Weapon, "rightHand" },
            { Classificator.HeroItemCategory.Armor, "body" },
            { Classificator.HeroItemCategory.Horse, "horse" },
            { Classificator.HeroItemCategory.Boots, "shoes" },
            { Classificator.HeroItemCategory.Others, "bag" }
        };
        /// <summary>
        /// Parses what items is hero currently equipt with
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>Equipt items</returns>
        public static Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum> GetHeroEquipment(HtmlDocument html)
        {
            var ret = new Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum>();

            foreach (var pair in HeroTypeIds)
            {
                var item = html.GetElementbyId(pair.Value).ChildNodes.FirstOrDefault(x => x.HasClass("item"));
                if (item == null) continue;

                (Classificator.HeroItemEnum? heroItemEnum, int amount) = ParseItemNode(item);
                if (heroItemEnum == null) continue;

                var itemEnum = heroItemEnum ?? Classificator.HeroItemEnum.Others_None_0;
                ret.Add(pair.Key, itemEnum);
            }
            return ret;
        }

        private static (Classificator.HeroItemEnum?, int) ParseItemNode(HtmlNode node)
        {
            var itemClass = node.GetClasses().FirstOrDefault(x => x.Contains("_item_"));
            if (itemClass == null) return (null, 0);

            var itemEnum = (Classificator.HeroItemEnum)Parser.RemoveNonNumeric(itemClass.Split('_').LastOrDefault());

            // Get amount
            var amount = node.ChildNodes.FirstOrDefault(x => x.HasClass("amount"));
            if (amount == null) return (itemEnum, 0);

            var amountNum = (int)Parser.RemoveNonNumeric(amount.InnerText);

            return (itemEnum, amountNum);
        }
    }
}
