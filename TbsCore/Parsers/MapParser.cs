using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Web;
using TbsCore.Models.MapModels;

namespace TbsCore.Parsers
{
    public static class MapParser
    {
        public static Coordinates GetCoordinates(HtmlNode node) =>
            GetCoordinates(node.Descendants("td").FirstOrDefault(x => x.HasClass("coords")).InnerText);

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

        /// <summary>
        /// Parse href="position_details.php?x=23&y=-45" as it appears in the FL rows
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Coordinates GetPositionDetails(HtmlNode node)
        {
            var href = node.Descendants("a").FirstOrDefault(x =>
                x.GetAttributeValue("href", "").StartsWith("position_details"));
            if (href == null) return null;

            // position_details.php?x=39&amp;y=-66
            var str = WebUtility.HtmlDecode(href.GetAttributeValue("href", ""));//.Split("");
            var par = HttpUtility.ParseQueryString(str.Split('?').Last());
            var X = int.Parse(par.Get("x"));
            var Y = int.Parse(par.Get("y"));
            return new Coordinates(X, Y);
        }
    }
}