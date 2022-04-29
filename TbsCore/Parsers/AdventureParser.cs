using System.Collections.Generic;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.Models;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Parsers
{
    public static class AdventureParser
    {
        public static int GetAdventureCount(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var adventureButton = htmlDoc.DocumentNode.Descendants().Where(x => x.Attributes.Any(a => a.Value.Contains("layoutButton adventureWhite green"))).FirstOrDefault();
            string count = adventureButton.Descendants().Where(x => x.Attributes.Any(a => a.Value == "speechBubbleContent")).FirstOrDefault().InnerText;
            return (int)Parser.ParseNum(count);
        }

        public static List<Adventure> GetAdventures(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            List<Adventure> adventuresList = new List<Adventure>();
            var adventures = htmlDoc.GetElementbyId("adventureListForm");
            if (adventures == null) return adventuresList;
            foreach (var adv in adventures.Descendants("tr"))
            {
                if (string.IsNullOrEmpty(adv.Id)) continue;
                var sec = (int)TimeParser.ParseDuration(adv.Descendants("td").FirstOrDefault(x => x.HasClass("moveTime")).InnerText).TotalSeconds;
                var coordinates = MapParser.GetCoordinates(adv);

                DifficultyEnum difficulty = DifficultyEnum.Normal;
                switch (version)
                {
                    case ServerVersionEnum.TTwars:
                        difficulty = adv.Descendants("img").FirstOrDefault().GetAttributeValue("alt", "") == "Normal" ?
                            DifficultyEnum.Normal : DifficultyEnum.Difficult;
                        break;

                    case ServerVersionEnum.T4_5:
                        difficulty = adv.Descendants("img").FirstOrDefault().GetAttributeValue("class", "") == "adventureDifficulty1" ?
                            DifficultyEnum.Normal : DifficultyEnum.Difficult;
                        break;
                }

                var secStr = adv.Descendants("td").FirstOrDefault(x => x.HasClass("timeLeft"))?.InnerText;
                int secRemaining = int.MaxValue;
                if (!string.IsNullOrEmpty(secStr)) secRemaining = (int)TimeParser.ParseDuration(secStr).TotalSeconds;

                switch (version)
                {
                    case Classificator.ServerVersionEnum.TTwars:
                        var href = adv.Descendants("a").FirstOrDefault(x => x.HasClass("gotoAdventure")).GetAttributeValue("href", "").Replace("amp;", "");
                        adventuresList.Add(new Adventure()
                        {
                            Coordinates = coordinates,
                            DurationSeconds = sec,
                            TimeLeftSeconds = secRemaining,
                            Difficulty = difficulty,
                            Ref = href
                        });
                        break;

                    case Classificator.ServerVersionEnum.T4_5:
                        var elementId = adv.Descendants("td").FirstOrDefault(x => x.HasClass("goTo")).Id;
                        adventuresList.Add(new Adventure()
                        {
                            Coordinates = coordinates,
                            DurationSeconds = sec,
                            TimeLeftSeconds = secRemaining,
                            Difficulty = difficulty,
                            AdventureId = elementId
                        });
                        break;
                }
            }
            return adventuresList;
        }
    }
}