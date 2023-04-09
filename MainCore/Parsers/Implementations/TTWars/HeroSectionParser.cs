using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TTWars
{
    public class HeroSectionParser : IHeroSectionParser
    {
        public int GetHealth(HtmlDocument doc)
        {
            var healthMask = doc.DocumentNode.Descendants("svg").FirstOrDefault(x => x.HasClass("health"));
            if (healthMask is null) return -1;
            var path = healthMask.Descendants("path").FirstOrDefault();
            if (path is null) return -1;
            var commands = path.GetAttributeValue("d", "").Split(' ');
            try
            {
                var xx = double.Parse(commands[^2], System.Globalization.CultureInfo.InvariantCulture);
                var yy = double.Parse(commands[^1], System.Globalization.CultureInfo.InvariantCulture);

                var rad = Math.Atan2(yy - 55, xx - 55);
                return (int)Math.Round(-56.173 * rad + 96.077);
            }
            catch
            {
                return 0;
            }
        }

        public int GetStatus(HtmlDocument doc)
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

        public int GetAdventureNum(HtmlDocument doc)
        {
            var adv45 = doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("adventure"));
            if (adv45 is null) return 0;
            var content = adv45.Descendants("div").FirstOrDefault(x => x.HasClass("content"));
            if (content is null) return 0;
            var valueStr = new string(content.InnerText.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 0;
            return int.Parse(valueStr);
        }

        public List<(int, int)> GetItems(HtmlDocument doc)
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

        public bool IsCurrentTab(HtmlNode tabNode)
        {
            return tabNode.HasClass("active");
        }

        public HtmlNode GetHeroInventory(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroImageButton");
        }

        public HtmlNode GetAdventuresButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("adventure"));
        }

        public List<HtmlNode> GetAdventures(HtmlDocument doc)
        {
            var adventures = doc.GetElementbyId("adventureListForm");
            if (adventures is null) return null;
            var list = adventures.Descendants("tr").ToList();
            list.RemoveAt(0);
            return list;
        }

        public int GetAdventureDifficult(HtmlNode node)
        {
            var img = node.Descendants("img").FirstOrDefault();
            if (img is null) return 0;
            var value = img.GetAttributeValue("alt", "");
            if (value.Equals("Normal")) return 0;
            return 1;
        }

        public (int, int) GetAdventureCoordinates(HtmlNode node)
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

        public HtmlNode GetHeroAvatar(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroImageButton");
        }

        public HtmlNode GetHeroTab(HtmlDocument doc, int index)
        {
            var heroDiv = doc.GetElementbyId("content");
            if (heroDiv is null) return null;
            var aNode = heroDiv.Descendants("a").ToList();
            if (index >= aNode.Count) return null;
            return aNode[index];
        }

        public HtmlNode GetItemSlot(HtmlDocument doc, int type)
        {
            var inventory = doc.GetElementbyId("itemsToSale");
            foreach (var itemSlot in inventory.ChildNodes)
            {
                var item = itemSlot.ChildNodes.FirstOrDefault(x => x.Id.StartsWith("item_"));
                if (item is null) continue;

                var itemClass = item.GetClasses().FirstOrDefault(x => x.Contains("_item_"));
                var itemValue = itemClass.Split('_').LastOrDefault();
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                if (int.Parse(itemValueStr) == type) return item;
            }
            return null;
        }

        public HtmlNode GetAmountBox(HtmlDocument doc)
        {
            return doc.GetElementbyId("amount");
        }

        public HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("ok"));
        }

        public HtmlNode GetStartAdventureButton(HtmlDocument doc, int x, int y)
        {
            var adventures = GetAdventures(doc);
            foreach (var adventure in adventures)
            {
                (var X, var Y) = GetAdventureCoordinates(adventure);
                if (X == x && Y == y) return adventure.ChildNodes.Descendants("a").Last();
            }
            return null;
        }
    }
}