using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialNewHeroUICore.Parsers
{
    public static class Hero
    {
        public static int GetHealth(HtmlDocument doc)
        {
            var healthMask = doc.GetElementbyId("healthMask");
            if (healthMask is null) return -1;
            var path = healthMask.Descendants("path").FirstOrDefault();
            if (path is null) return -1;
            var commands = path.GetAttributeValue("d", "").Split(' ');
            var xx = double.Parse(commands[^2], System.Globalization.CultureInfo.InvariantCulture);
            var yy = double.Parse(commands[^1], System.Globalization.CultureInfo.InvariantCulture);

            var rad = Math.Atan2(yy - 55, xx - 55);
            return (int)Math.Round(-56.173 * rad + 96.077);
        }

        public static int GetStatus(HtmlDocument doc)
        {
            var heroStatusDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatus"));
            if (heroStatusDiv is null) return 0;
            var iconHeroStatus = heroStatusDiv.Descendants("i").FirstOrDefault();
            if (iconHeroStatus == null) return 0;
            var status = iconHeroStatus.GetClasses().FirstOrDefault();
            if (status is null) return 0;
            return status switch
            {
                "heroRunning" => 2,// away
                "heroHome" => 1,// home
                "heroDead" => 3,// dead
                "heroReviving" => 4,// regenerating
                "heroReinforcing" => 5,// reinforcing
                _ => 0,
            };
        }

        public static int GetAdventureNum(HtmlDocument doc)
        {
            var adv45 = doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("adventure"));
            if (adv45 is null) return -1;
            var content = adv45.Descendants("div").FirstOrDefault(x => x.HasClass("content"));
            if (content is null) return -1;
            var valueStr = new string(content.InnerText.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return -1;
            return int.Parse(valueStr);
        }

        public static List<(int, int)> GetItems(HtmlDocument doc)
        {
            var heroItems = new List<(int, int)>();
            var heroItemsDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroItems"));
            if (heroItemsDiv is null) return null;
            var heroItemDivs = heroItemsDiv.Descendants("div").Where(x => x.HasClass("heroItem") && !x.HasClass("empty"));
            if (!heroItemDivs.Any()) return null;

            foreach (var itemSlot in heroItemDivs)
            {
                if (itemSlot.ChildNodes.Count != 2) continue;
                var itemNode = itemSlot.ChildNodes[1];
                var classes = itemNode.GetClasses();
                if (classes.Count() != 2) continue;

                var itemValue = classes.ElementAt(1);
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                if (itemSlot.GetAttributeValue("data-tier", "").Contains("consumable"))
                {
                    if (itemSlot.ChildNodes.Count != 3)
                    {
                        heroItems.Add((int.Parse(itemValueStr), 1));
                        continue;
                    }
                    var amountNode = itemSlot.ChildNodes[2];

                    var amountValueStr = new string(amountNode.InnerText.Where(c => char.IsDigit(c)).ToArray());
                    if (string.IsNullOrEmpty(amountValueStr))
                    {
                        heroItems.Add((int.Parse(itemValueStr), 1));
                        continue;
                    }
                    heroItems.Add((int.Parse(itemValueStr), int.Parse(amountValueStr)));
                }
                else
                {
                    heroItems.Add((int.Parse(itemValueStr), 1));
                }
            }
            return heroItems;
        }
    }
}