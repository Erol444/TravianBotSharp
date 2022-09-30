using HtmlAgilityPack;
using System.Linq;
using TravianOfficialNewHeroUICore.Parsers;

namespace TravianOfficialNewHeroUICore.FindElements
{
    public static class HeroPage
    {
        public static HtmlNode GetHeroAvatar(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroImageButton");
        }

        public static HtmlNode GetHeroTab(HtmlDocument doc, int index)
        {
            var heroDiv = doc.GetElementbyId("heroV2");
            if (heroDiv is null) return null;
            var aNode = heroDiv.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("data-tab", 0) == index);
            return aNode;
        }

        public static HtmlNode GetAdventuresButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("adventure"));
        }

        public static HtmlNode GetItemSlot(HtmlDocument doc, int type)
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

        public static HtmlNode GetAmountBox(HtmlDocument doc)
        {
            var form = doc.GetElementbyId("consumableHeroItem");
            return form.Descendants("input").FirstOrDefault();
        }

        public static HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            var dialog = doc.GetElementbyId("dialogContent");
            var buttonWrapper = dialog.Descendants("div").FirstOrDefault(x => x.HasClass("buttonsWrapper"));
            var buttonTransfer = buttonWrapper.Descendants("button");
            if (buttonTransfer.Count() < 2) return null;
            return buttonTransfer.ElementAt(1);
        }

        public static HtmlNode GetStartAdventureButton(HtmlDocument doc, int x, int y)
        {
            var adventures = HeroInfo.GetAdventures(doc);
            foreach (var adventure in adventures)
            {
                (var X, var Y) = HeroInfo.GetAdventureCoordinates(adventure);
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