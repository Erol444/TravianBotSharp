using HtmlAgilityPack;
using System.Linq;
using TravianOfficialCore.Parsers;

namespace TravianOfficialCore.FindElements
{
    public static class HeroPage
    {
        public static HtmlNode GetHeroInventory(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroImageButton");
        }

        public static HtmlNode GetAdventuresButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("adventure"));
        }

        public static HtmlNode GetItemSlot(HtmlDocument doc, int type)
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

        public static HtmlNode GetAmountBox(HtmlDocument doc)
        {
            return doc.GetElementbyId("amount");
        }

        public static HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("ok"));
        }

        public static HtmlNode GetStartAdventureButton(HtmlDocument doc, int x, int y)
        {
            var adventures = HeroInfo.GetAdventures(doc);
            foreach (var adventure in adventures)
            {
                (var X, var Y) = HeroInfo.GetAdventureCoordinates(adventure);
                if (X == x && Y == y)
                {
                    return adventure.Descendants("a").Last();
                }
            }
            return null;
        }
    }
}