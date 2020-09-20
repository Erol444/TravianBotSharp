using HtmlAgilityPack;
using System;
using System.Linq;

namespace TravBotSharp.Files.Parsers
{
    public static class TimeParser
    {
        public static TimeSpan ParseDuration(HtmlNode node)
        {
            var duration = node.Descendants().FirstOrDefault(x => x.HasClass("duration") || x.HasClass("clocks") || x.HasClass("value"));
            return ParseDuration(duration.InnerText);
        }

        public static TimeSpan ParseDuration(string str)
        {
            //00:00:02 (+332 ms), TTWars, milliseconds matter
            int ms = 0;
            if (str.Contains("(+"))
            {
                var parts = str.Split('(');
                ms = (int)Parser.RemoveNonNumeric(parts[1]);
                str = parts[0];
            }
            // h:m:s
            var arr = str.Split(':');
            var h = (int)Parser.RemoveNonNumeric(arr[0]);
            var m = (int)Parser.RemoveNonNumeric(arr[1]);
            var s = (int)Parser.RemoveNonNumeric(arr[2]);
            return new TimeSpan(0, h, m, s, ms);
        }

        /// <summary>
        /// Parses timer. Will search for descendants that have class name "timer"
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan ParseTimer(HtmlNode node)
        {
            var timer = node.Descendants().FirstOrDefault(x => x.HasClass("timer"));
            if (timer == null) return TimeSpan.Zero;
            int sec = int.Parse(timer.GetAttributeValue("value", "0"));
            if (sec < 0) sec = 0;
            return TimeSpan.FromSeconds(sec);
        }

        public static DateTime GetServerTime(HtmlDocument html)
        {
            var serverTime = html.GetElementbyId("servertime");
            var timer = serverTime.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));

            var dur = TimeParser.ParseDuration(timer.InnerText);
            return DateTime.Today.Add(dur);
        }
    }
}
