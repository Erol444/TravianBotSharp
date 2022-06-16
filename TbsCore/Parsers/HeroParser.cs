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
        private static readonly Classificator.HeroItemCategory[] heroItemCategories = new Classificator.HeroItemCategory[7] {
            Classificator.HeroItemCategory.Helmet,
            Classificator.HeroItemCategory.Armor,
            Classificator.HeroItemCategory.Boots,
            Classificator.HeroItemCategory.Left,
            Classificator.HeroItemCategory.Weapon,
            Classificator.HeroItemCategory.Horse,
            Classificator.HeroItemCategory.Stackable,
        };

        public static HeroInfo GetHeroAttributes(HtmlDocument htmlDoc)
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

        public static List<HeroItem> GetHeroInventory(HtmlDocument htmlDoc)
        {
            List<HeroItem> heroItems = new List<HeroItem>();
            var heroItemsDiv = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroItems"));
            var heroItemDivs = heroItemsDiv.Descendants("div").Where(x => x.HasClass("heroItem") && !x.HasClass("empty"));
            foreach (var itemSlot in heroItemDivs)
            {
                (var heroItemEnum, int amount) = ParseItemNode(itemSlot);

                var heroItem = new HeroItem
                {
                    Item = heroItemEnum,
                    Count = amount
                };

                heroItems.Add(heroItem);
            }
            return heroItems;
        }

        public static Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum> GetHeroEquipment(HtmlDocument htmlDoc)
        {
            var ret = new Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum>();
            var equipmentSlotsDiv = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("equipmentSlots"));
            var heroItemDivs = equipmentSlotsDiv.Descendants("div").Where(x => x.HasClass("heroItem")).ToArray();

            for (var i = 0; i < 7; i++)
            {
                var slot = heroItemDivs[i];
                if (slot.HasClass("empty"))
                {
                    ret.Add(heroItemCategories[i], Classificator.HeroItemEnum.Others_None_0);
                }
                else
                {
                    (var heroItemEnum, _) = ParseItemNode(slot);
                    ret.Add(heroItemCategories[i], heroItemEnum);
                }
            }
            return ret;
        }

        /// <summary>
        /// Parses when the hero arrival will be (parsed from /hero.php)
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>TimeSpan after how much time hero arrival will happen</returns>
        public static TimeSpan GetHeroArrivalInfo(HtmlDocument html)
        {
            var statusMsg = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
            if (statusMsg == null) return new TimeSpan(-1, 0, 0); // -1 hour

            return TimeParser.ParseTimer(statusMsg);
        }

        public static int GetAdventureNum(HtmlDocument htmlDoc)
        {
            var adv45 = htmlDoc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("adventure"));
            var num = adv45.ChildNodes.FirstOrDefault(x => x.HasClass("content") && x.Name == "div");
            if (num == null) return 0;
            return (int)Parser.RemoveNonNumeric(num.InnerText);
        }

        public static bool LeveledUp(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode
                .Descendants("i")
                .Any(x => x.HasClass("levelUp") && x.HasClass("show"));
        }

        public static Hero.StatusEnum HeroStatus(HtmlDocument htmlDoc)
        {
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

                default:
                    return Hero.StatusEnum.Unknown;
            }
        }

        public static int GetHeroHealth(HtmlDocument htmlDoc)
        {
            var path = htmlDoc.GetElementbyId("healthMask").Descendants("path").FirstOrDefault();
            if (path == null) return 0;
            var commands = path.GetAttributeValue("d", "").Split(' ');
            try
            {
                var xx = double.Parse(commands[commands.Length - 2], System.Globalization.CultureInfo.InvariantCulture);
                var yy = double.Parse(commands[commands.Length - 1], System.Globalization.CultureInfo.InvariantCulture);

                var rad = Math.Atan2(yy - 55, xx - 55);
                return (int)Math.Round(-56.173 * rad + 96.077);
            }
            catch
            {
                return 100;
            }
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

        private static readonly Dictionary<Classificator.HeroItemCategory, string> HeroTypeIds = new Dictionary<Classificator.HeroItemCategory, string>()
        {
            { Classificator.HeroItemCategory.Helmet, "helmet" },
            { Classificator.HeroItemCategory.Left, "leftHand" },
            { Classificator.HeroItemCategory.Weapon, "rightHand" },
            { Classificator.HeroItemCategory.Armor, "body" },
            { Classificator.HeroItemCategory.Horse, "horse" },
            { Classificator.HeroItemCategory.Boots, "shoes" }
        };

        public static (Classificator.HeroItemEnum, int) ParseItemNode(HtmlNode node)
        {
            var itemNode = node.ChildNodes[1];
            var itemEnum = (Classificator.HeroItemEnum)Parser.RemoveNonNumeric(itemNode.GetClasses().ToArray()[1]);
            var amountNum = 1;
            if (node.GetAttributeValue("data-tier", "").Contains("consumable"))
            {
                var amountNode = node.ChildNodes[2];
                amountNum = (int)Parser.RemoveNonNumeric(amountNode.InnerText);
            }

            return (itemEnum, amountNum);
        }
    }
}