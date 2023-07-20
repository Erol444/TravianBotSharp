using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TravianOfficial
{
    public class HeroSectionParser : IHeroSectionParser
    {
        public int GetHealth(HtmlDocument doc)
        {
            var healthMask = doc.GetElementbyId("healthMask");
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
            var heroItemsDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroItems"));
            if (heroItemsDiv is null) return heroItems;
            var heroItemDivs = heroItemsDiv.Descendants("div").Where(x => x.HasClass("heroItem") && !x.HasClass("empty"));
            if (!heroItemDivs.Any()) return heroItems;

            foreach (var itemSlot in heroItemDivs)
            {
                if (itemSlot.ChildNodes.Count < 2) continue;
                var itemNode = itemSlot.ChildNodes[1];
                var classes = itemNode.GetClasses();
                if (classes.Count() != 2) continue;

                var itemValue = classes.ElementAt(1);
                if (itemValue is null) continue;

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                if (itemSlot.GetAttributeValue("data-tier", "").Contains("consumable"))
                {
                    if (itemSlot.ChildNodes.Count < 3)
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

        public bool IsCurrentTab(HtmlNode tabNode)
        {
            return tabNode.HasClass("active");
        }

        public HtmlNode GetAdventuresButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("adventure"));
        }

        public List<HtmlNode> GetAdventures(HtmlDocument doc)
        {
            var adventures = doc.GetElementbyId("heroAdventure");
            if (adventures is null) return null;
            var tbody = adventures.Descendants("tbody").FirstOrDefault();
            if (tbody is null) return null;

            return tbody.Descendants("tr").ToList();
        }

        public int GetAdventureDifficult(HtmlNode node)
        {
            var tdList = node.Descendants("td").ToArray();
            if (tdList.Length < 3) return 0;
            var iconDifficulty = tdList[3].FirstChild;
            if (iconDifficulty.GetAttributeValue("alt", "").Contains("hard")) return 1;
            return 0;
        }

        public (int, int) GetAdventureCoordinates(HtmlNode node)
        {
            var tdList = node.Descendants("td").ToArray();
            if (tdList.Length < 2) return (0, 0);
            var coords = tdList[1].InnerText.Split('|');
            if (coords.Length < 2) return (0, 0);
            coords[0] = coords[0].Replace('−', '-');
            var valueX = new string(coords[0].Where(c => char.IsDigit(c) || c == '-').ToArray());
            if (string.IsNullOrEmpty(valueX)) return (0, 0);
            coords[1] = coords[1].Replace('−', '-');
            var valueY = new string(coords[1].Where(c => char.IsDigit(c) || c == '-').ToArray());
            if (string.IsNullOrEmpty(valueY)) return (0, 0);
            return (int.Parse(valueX), int.Parse(valueY));
        }

        public HtmlNode GetHeroAvatar(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroImageButton");
        }

        public HtmlNode GetHeroTab(HtmlDocument doc, int index)
        {
            var heroDiv = doc.GetElementbyId("heroV2");
            if (heroDiv is null) return null;
            var aNode = heroDiv.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("data-tab", 0) == index);
            return aNode;
        }

        public HtmlNode GetItemSlot(HtmlDocument doc, int type)
        {
            var heroItemsDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroItems"));
            if (heroItemsDiv is null) return null;
            var heroItemDivs = heroItemsDiv.Descendants("div").Where(x => x.HasClass("heroItem") && !x.HasClass("empty"));
            if (!heroItemDivs.Any()) return null;

            foreach (var itemSlot in heroItemDivs)
            {
                if (itemSlot.ChildNodes.Count < 2) continue;
                var itemNode = itemSlot.ChildNodes[1];
                var classes = itemNode.GetClasses();
                if (classes.Count() != 2) continue;

                var itemValue = classes.ElementAt(1);

                var itemValueStr = new string(itemValue.Where(c => char.IsDigit(c)).ToArray());
                if (string.IsNullOrEmpty(itemValueStr)) continue;

                if (int.Parse(itemValueStr) == type) return itemSlot;
            }
            return null;
        }

        public HtmlNode GetAmountBox(HtmlDocument doc)
        {
            var form = doc.GetElementbyId("consumableHeroItem");
            return form.Descendants("input").FirstOrDefault();
        }

        public HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            var dialog = doc.GetElementbyId("dialogContent");
            var buttonWrapper = dialog.Descendants("div").FirstOrDefault(x => x.HasClass("buttonsWrapper"));
            var buttonTransfer = buttonWrapper.Descendants("button");
            if (buttonTransfer.Count() < 2) return null;
            return buttonTransfer.ElementAt(1);
        }

        public HtmlNode GetStartAdventureButton(HtmlDocument doc, int x, int y)
        {
            var adventures = GetAdventures(doc);
            foreach (var adventure in adventures)
            {
                (var X, var Y) = GetAdventureCoordinates(adventure);
                if (X == x && Y == y)
                {
                    var last = adventure.ChildNodes.Last();
                    return last.Descendants("button").FirstOrDefault();
                }
            }
            return null;
        }
    }
}