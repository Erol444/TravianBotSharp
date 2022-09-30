using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace TTWarsCore.Parsers
{
    public static class HeroInfo
    {
        public static int GetHealth(HtmlDocument doc)
        {
            var sidebarBox = doc.GetElementbyId("sidebarBoxHero");
            if (sidebarBox is null) return -1;
            var healthBar = sidebarBox.Descendants("div").FirstOrDefault(x => x.HasClass("bar"));
            if (healthBar is null) return -1;
            try
            {
                var health = healthBar.Attributes.FirstOrDefault(x => x.Name == "style").Value.Split(':')[1].Replace("%", "");
                health = health.Split('.')[0];
                var valueStr = new string(health.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(valueStr)) return -1;
                return int.Parse(valueStr);
            }
            catch
            {
                return -1;
            }
        }

        public static int GetStatus(HtmlDocument doc)
        {
            var message = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroStatusMessage"));
            if (message is null) return 0;
            var img = message.ChildNodes.FirstOrDefault(x => x.Name == "img");
            if (img is null) return 0;
            if (img.HasClass("heroStatus101Regenerate"))
            {
                return 4; // Regenerating
            }
            else if (img.HasClass("heroStatus101"))
            {
                return 3; // Dead
            }
            else if (img.HasClass("heroStatus100"))
            {
                return 1; // Home
            }
            else if (img.HasClass("heroStatus50"))
            {
                return 2; // Away
            }
            return 0;
        }

        public static int GetAdventureNum(HtmlDocument doc)
        {
            var adv45 = doc.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("adventureWhite"));
            if (adv45 is null) return 0;
            var content = adv45.Descendants().FirstOrDefault(x => x.HasClass("speechBubbleContent"));
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
            var value = img.GetAttributeValue("alt", "");
            if (value.Equals("Normal")) return 0;
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