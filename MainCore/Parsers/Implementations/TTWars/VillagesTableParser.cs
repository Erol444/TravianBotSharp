using HtmlAgilityPack;
using MainCore.Parser.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parser.Implementations.TTWars
{
    public class VillagesTableParser : IVillagesTableParser
    {
        public List<HtmlNode> GetVillages(HtmlDocument doc)
        {
            var villsNode = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.Id.Equals("sidebarBoxVillagelist"));
            if (villsNode is null) return null;
            return villsNode.Descendants("li").ToList();
        }

        public bool IsUnderAttack(HtmlNode node)
        {
            return node.HasClass("attack");
        }

        public bool IsActive(HtmlNode node)
        {
            return node.HasClass("active");
        }

        public int GetId(HtmlNode node)
        {
            var hrefNode = node.ChildNodes.FirstOrDefault(x => x.Name == "a");
            if (hrefNode is null) return -1;
            var href = System.Net.WebUtility.HtmlDecode(hrefNode.GetAttributeValue("href", ""));
            if (string.IsNullOrEmpty(href)) return -1;
            if (!href.Contains('=')) return -1;
            var value = href.Split('=')[1];
            if (value.Contains('&'))
            {
                value = value.Split('&')[0];
            }
            return int.Parse(value);
        }

        public string GetName(HtmlNode node)
        {
            var textNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("name"));
            if (textNode is null) return "";
            return textNode.InnerText;
        }

        public int GetX(HtmlNode node)
        {
            var xNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateX"));
            if (xNode is null) return 0;
            var xStr = new string(xNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('-')).ToArray());
            if (string.IsNullOrEmpty(xStr)) return 0;
            return int.Parse(xStr);
        }

        public int GetY(HtmlNode node)
        {
            var yNode = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateY"));
            if (yNode is null) return 0;
            var yStr = new string(yNode.InnerText.Where(c => char.IsDigit(c) || c.Equals('-')).ToArray());
            if (string.IsNullOrEmpty(yStr)) return 0;

            return int.Parse(yStr);
        }
    }
}