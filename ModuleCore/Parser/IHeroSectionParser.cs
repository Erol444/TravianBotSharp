using HtmlAgilityPack;
using System.Collections.Generic;

namespace ModuleCore.Parser
{
    public interface IHeroSectionParser
    {
        public int GetHealth(HtmlDocument doc);

        public int GetStatus(HtmlDocument doc);

        public int GetAdventureNum(HtmlDocument doc);

        public HtmlNode GetHeroAvatar(HtmlDocument doc);

        public HtmlNode GetHeroTab(HtmlDocument doc, int index);

        public bool IsCurrentTab(HtmlNode tabNode);

        public HtmlNode GetAdventuresButton(HtmlDocument doc);

        public List<HtmlNode> GetAdventures(HtmlDocument doc);

        public int GetAdventureDifficult(HtmlNode node);

        public (int, int) GetAdventureCoordinates(HtmlNode node);

        public List<(int, int)> GetItems(HtmlDocument doc);

        public HtmlNode GetItemSlot(HtmlDocument doc, int type);

        public HtmlNode GetAmountBox(HtmlDocument doc);

        public HtmlNode GetConfirmButton(HtmlDocument doc);

        public HtmlNode GetStartAdventureButton(HtmlDocument doc, int x, int y);
    }
}