using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TravianOfficialCore.Parsers
{
    public static class HeroInfo
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
            var statusNode = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatus"));
            if (statusNode is null) return 0;
            var img = statusNode.Descendants().FirstOrDefault(x => x.Name == "svg");
            if (img is null) return 0;
            var status = img.GetClasses().FirstOrDefault();
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
            if (adv45 is null) return 0;
            var content = adv45.Descendants("div").FirstOrDefault(x => x.HasClass("content"));
            if (content is null) return 0;
            var valueStr = new string(content.InnerText.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 0;
            return int.Parse(valueStr);
        }

        public static List<(int, int)> GetItems(HtmlDocument doc)
        {
            var heroItems = new List<(int, int)>();
            var inventory = doc.GetElementbyId("itemsToSale");
            if (inventory is null) return null;

            foreach (var itemSlot in inventory.ChildNodes)
            {
                var item = itemSlot.ChildNodes.FirstOrDefault(x => x.Id.StartsWith("item_"));
                if (item is null) continue;

                var itemClass = item.GetClasses().FirstOrDefault(x => x.Contains("_item_"));
                var itemValue = itemClass.Split('_').LastOrDefault();
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                var amountValue = item.ChildNodes.FirstOrDefault(x => x.HasClass("amount"));
                if (amountValue is null)
                {
                    heroItems.Add((int.Parse(itemValueStr), 1));
                    continue;
                }

                var amountValueStr = new string(amountValue.InnerText.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr))
                {
                    heroItems.Add((int.Parse(itemValueStr), 1));
                    continue;
                }

                heroItems.Add((int.Parse(itemValueStr), int.Parse(amountValueStr)));
            }
            return heroItems;
        }

        public static List<HtmlNode> GetAdventures(HtmlDocument doc)
        {
            var adventures = doc.GetElementbyId("adventureListForm");
            if (adventures is null) return null;
            var list = adventures.Descendants("tr").ToList();
            list.RemoveAt(0);
            return list;
        }

        public static int GetAdventureDifficult(HtmlNode node)
        {
            var img = node.Descendants("img").FirstOrDefault();
            if (img is null) return 0;
            if (img.HasClass("adventureDifficulty1")) return 0;
            return 1;
        }

        public static (int, int) GetAdventureCoordinates(HtmlNode node)
        {
            var coordsNode = node.Descendants("td").FirstOrDefault(x => x.HasClass("coords"));
            if (coordsNode is null) return (0, 0);
            var coords = coordsNode.InnerText.Split('|');
            if (coords.Length < 2) return (0, 0);
            var valueX = new string(coords[0].Where(c => char.IsDigit(c) || c == '−').ToArray());
            if (string.IsNullOrEmpty(valueX)) return (0, 0);
            var valueY = new string(coords[1].Where(c => char.IsDigit(c) || c == '−').ToArray());
            if (string.IsNullOrEmpty(valueY)) return (0, 0);
            valueX = valueX.Replace('−', '-');
            valueY = valueY.Replace('−', '-');
            return (int.Parse(valueX), int.Parse(valueY));
        }
    }
}