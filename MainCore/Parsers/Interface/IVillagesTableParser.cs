using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface IVillagesTableParser
    {
        public List<HtmlNode> GetVillages(HtmlDocument doc);

        public bool IsUnderAttack(HtmlNode node);

        public bool IsActive(HtmlNode node);

        public int GetId(HtmlNode node);

        public string GetName(HtmlNode node);

        public int GetX(HtmlNode node);

        public int GetY(HtmlNode node);
    }
}