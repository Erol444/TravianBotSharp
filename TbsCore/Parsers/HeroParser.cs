using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;

namespace TbsCore.Parsers
{
    public static class HeroParser
    {
        public static bool AttributesHidden(HtmlDocument htmlDoc)
        {
            var attributes = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroPropertiesContent"));
            if (attributes?.GetClasses()?.FirstOrDefault(x => x == "hide") == null) return false;
            else return true;
        }

        public static HeroInfo GetHeroInfo(HtmlDocument htmlDoc)
        {
            var heroInfo = new HeroInfo();
            {
                var heroLevel = htmlDoc.DocumentNode.Descendants()
                    .FirstOrDefault(x => x.HasClass("titleInHeader"))
                    .InnerText
                    .Split('-')
                    .Last();
                heroInfo.Level = (int)Parser.RemoveNonNumeric(heroLevel);
            }

            {
                var statsDiv = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("stats"));
                var valueDivs = statsDiv.Descendants("div").Where(x => x.HasClass("value")).ToArray();
                heroInfo.Health = (int)Parser.ParseNum(valueDivs[0].InnerText.Replace("%", ""));
                heroInfo.Experience = (int)Parser.RemoveNonNumeric(valueDivs[1].InnerText);
            }

            {
                var productionDiv = htmlDoc.DocumentNode.Descendants("div").LastOrDefault(x => x.HasClass("productionItem"));
                heroInfo.HeroProduction = (int)Parser.RemoveNonNumeric(productionDiv.InnerText);

                var changeProductionDiv = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("changeProduction"));
                var buttons = changeProductionDiv.Descendants("button").ToArray();

                for (var i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].HasClass("active"))
                    {
                        heroInfo.SelectedResource = i;
                        break;
                    }
                }
            }
            {
                var attributesDiv = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroAttributes"));
                var avaliblePoints = attributesDiv.Descendants("div").FirstOrDefault(x => x.HasClass("pointsAvailable"));
                heroInfo.AvaliblePoints = (int)Parser.RemoveNonNumeric(avaliblePoints.InnerText);
                if (heroInfo.AvaliblePoints > 0) heroInfo.NewLevel = true;

                var pointInputs = attributesDiv.Descendants("input");

                var fightingStrengthInput = pointInputs.FirstOrDefault(x => x.GetAttributeValue("name", "").Contains("fightingStrength"));
                heroInfo.FightingStrengthPoints = fightingStrengthInput.GetAttributeValue("value", 0);

                var offBonusInput = pointInputs.FirstOrDefault(x => x.GetAttributeValue("name", "").Contains("offBonus"));
                heroInfo.OffBonusPoints = offBonusInput.GetAttributeValue("value", 0);

                var defBonusInput = pointInputs.FirstOrDefault(x => x.GetAttributeValue("name", "").Contains("defBonus"));
                heroInfo.DeffBonusPoints = defBonusInput.GetAttributeValue("value", 0);

                var resourceProductionInput = pointInputs.FirstOrDefault(x => x.GetAttributeValue("name", "").Contains("resourceProduction"));
                heroInfo.ResourcesPoints = resourceProductionInput.GetAttributeValue("value", 0);
            }

            return heroInfo;
        }

        /// <summary>
        /// Checks if hero is dead
        /// </summary>
        public static bool IsHeroDead(HtmlDocument html)
        {
            var status = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatusMessage"));
            if (status == null) return false;
            var img = status.Descendants("img").FirstOrDefault(x => x.HasClass("heroStatus101"));
            if (img == null) return false;
            return true;
        }

        /// <summary>
        /// Parses when the hero arrival will be (parsed from /hero.php)
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>TimeSpan after how much time hero arrival will happen</returns>
        public static TimeSpan GetHeroArrivalInfo(HtmlDocument html)
        {
            var statusMsg = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatusMessage"));
            if (statusMsg == null) return new TimeSpan(-1, 0, 0); // -1 hour

            return TimeParser.ParseTimer(statusMsg);
        }

        public static int GetAdventureNum(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.TTwars:
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

        public static bool LeveledUp(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    return htmlDoc.DocumentNode
                        .Descendants("div")
                        .Any(x => x.HasClass("levelUp"));

                case Classificator.ServerVersionEnum.T4_5:
                    return htmlDoc.DocumentNode
                        .Descendants("i")
                        .Any(x => x.HasClass("levelUp") && x.HasClass("show"));
            }
            return false;
        }

        public static Hero.StatusEnum HeroStatus(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.TTwars:
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
                    var heroStatusDiv = htmlDoc.DocumentNode.Descendants("div").First(x => x.HasClass("heroStatus"));
                    if (heroStatusDiv == null) return Hero.StatusEnum.Unknown;
                    var iconHeroStatus = heroStatusDiv.Descendants("i").FirstOrDefault();
                    if (iconHeroStatus == null) return Hero.StatusEnum.Unknown;
                    var str = iconHeroStatus.GetClasses().FirstOrDefault();
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

                        case "heroReinforcing":
                            return Hero.StatusEnum.Reinforcing;

                        default: return Hero.StatusEnum.Unknown;
                            //TODO ADD FOR DEAD, REGENERATING
                    }
            }
            return Hero.StatusEnum.Unknown;
        }

        public static int GetHeroHealth(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    var health = htmlDoc.GetElementbyId("sidebarBoxHero").Descendants("div").FirstOrDefault(x => x.HasClass("bar")).Attributes.FirstOrDefault(x => x.Name == "style").Value.Split(':')[1].Replace("%", "");
                    health = health.Split('.')[0];
                    return (int)Parser.RemoveNonNumeric(health);

                case Classificator.ServerVersionEnum.T4_5:
                    var path = htmlDoc.GetElementbyId("healthMask").Descendants("path").FirstOrDefault();
                    if (path == null) return 0;
                    var commands = path.GetAttributeValue("d", "").Split(' ');
                    var xx = double.Parse(commands[commands.Length - 2], System.Globalization.CultureInfo.InvariantCulture);
                    var yy = double.Parse(commands[commands.Length - 1], System.Globalization.CultureInfo.InvariantCulture);

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

            return MapParser.GetKarteHref(node);
        }

        public static int GetAvailablePoints(HtmlDocument htmlDoc)
        {
            var span = htmlDoc.GetElementbyId("availablePoints");
            var points = (int)Parser.RemoveNonNumeric(span.InnerText);
            return points;
        }

        public static TimeSpan GetHeroArrival(HtmlDocument htmlDoc)
        {
            var nodeStatus = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
            if (nodeStatus == null) return TimeSpan.Zero;
            return TimeParser.ParseTimer(nodeStatus);
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
            { Classificator.HeroItemCategory.Boots, "shoes" }
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

        public static (Classificator.HeroItemEnum?, int) ParseItemNode(HtmlNode node)
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