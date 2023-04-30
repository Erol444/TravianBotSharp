using HtmlAgilityPack;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TravianOfficial
{
    public class TrainTroopParser : ITrainTroopParser
    {
        public HtmlNode GetInputBox(HtmlNode node)
        {
            var cta = node.Descendants("div").FirstOrDefault(x => x.HasClass("cta"));
            var input = cta.Descendants("input").FirstOrDefault(x => x.HasClass("text"));
            return input;
        }

        public int GetMaxAmount(HtmlNode node)
        {
            var cta = node.Descendants("div").FirstOrDefault(x => x.HasClass("cta"));
            var a = cta.Descendants("a").FirstOrDefault();
            var value = new string(a.InnerText.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        public TimeSpan GetQueueTrainTime(HtmlDocument doc)
        {
            var table = doc.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            if (table is null) return TimeSpan.FromSeconds(0);
            var td = table.Descendants("td").FirstOrDefault(x => x.HasClass("dur"));
            var timer = td.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            var value = timer.GetAttributeValue("value", 0);
            return TimeSpan.FromSeconds(value);
        }

        public HtmlNode GetTrainButton(HtmlDocument doc)
        {
            return doc.GetElementbyId("s1");
        }

        public Resources GetTrainCost(HtmlNode node)
        {
            var resource = node.Descendants("div").Where(x => x.HasClass("inlineIcon") && x.HasClass("resource")).SkipLast(1).ToList();
            var wood = resource[0].Descendants("span").FirstOrDefault(x => x.HasClass("value")).InnerText;
            var woodValue = new string(wood.Where(c => char.IsDigit(c)).ToArray());

            var clay = resource[1].Descendants("span").FirstOrDefault(x => x.HasClass("value")).InnerText;
            var clayValue = new string(clay.Where(c => char.IsDigit(c)).ToArray());

            var iron = resource[2].Descendants("span").FirstOrDefault(x => x.HasClass("value")).InnerText;
            var ironValue = new string(iron.Where(c => char.IsDigit(c)).ToArray());

            var crop = resource[3].Descendants("span").FirstOrDefault(x => x.HasClass("value")).InnerText;
            var cropValue = new string(crop.Where(c => char.IsDigit(c)).ToArray());

            return new Resources()
            {
                Wood = int.Parse(woodValue),
                Clay = int.Parse(clayValue),
                Iron = int.Parse(ironValue),
                Crop = int.Parse(cropValue)
            };
        }

        public TimeSpan GetTrainTime(HtmlNode node)
        {
            var durationDiv = node.Descendants("div").FirstOrDefault(x => x.HasClass("duration"));
            var durationSpan = durationDiv.Descendants("span").FirstOrDefault(x => x.HasClass("value"));
            return durationSpan.InnerText.ToDuration();
        }

        public List<HtmlNode> GetTroopNodes(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("div").Where(x => x.HasClass("troop") && !x.HasClass("empty")).ToList();
        }

        public int GetTroopType(HtmlNode node)
        {
            var img = node.Descendants("img").FirstOrDefault(x => x.HasClass("unit"));
            var classes = img.GetClasses();
            var type = classes.FirstOrDefault(x => x.StartsWith("u") && !x.Equals("unit"));

            var value = new string(type.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }
    }
}