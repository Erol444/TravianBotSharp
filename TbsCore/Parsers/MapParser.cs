using HtmlAgilityPack;
using System;
using System.Linq;
using TbsCore.Models.MapModels;

namespace TbsCore.Parsers
{
    public static class MapParser
    {
        public static Coordinates GetCoordinates(string str)
        {
            var coords = str.Replace("(", "").Replace(")", "").Trim().Split('|');
            return new Coordinates() { x = (int)Parser.ParseNum(coords[0]), y = (int)Parser.ParseNum(coords[1]) };
        }

        public static int? GetKarteHref(HtmlNode node)
        {
            var href = node.Descendants("a").FirstOrDefault(x =>
                x.GetAttributeValue("href", "").StartsWith("/karte") ||
                x.GetAttributeValue("href", "").StartsWith("karte") // TTWars
            );
            if (href == null) return null;

            return Convert.ToInt32(href.GetAttributeValue("href", "").Split('=').Last());
        }
    }
}