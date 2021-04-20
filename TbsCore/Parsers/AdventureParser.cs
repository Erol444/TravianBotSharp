using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using TbsCore.Models;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Parsers
{
    public static class AdventureParser
    {
        public static int GetAdventureCount(HtmlDocument htmlDoc)
        {
            var adventureButton = htmlDoc.DocumentNode.Descendants()
                .Where(x => x.Attributes.Any(a => a.Value.Contains("layoutButton adventureWhite green")))
                .FirstOrDefault();
            var count = adventureButton.Descendants()
                .Where(x => x.Attributes.Any(a => a.Value == "speechBubbleContent")).FirstOrDefault().InnerText;
            return (int) Parser.ParseNum(count);
        }

        public static List<Adventure> GetAdventures(HtmlDocument htmlDoc, ServerVersionEnum version)
        {
            var adventuresList = new List<Adventure>();
            var adventures = htmlDoc.GetElementbyId("adventureListForm");
            if (adventures == null) return adventuresList;
            foreach (var adv in adventures.Descendants("tr"))
            {
                if (string.IsNullOrEmpty(adv.Id)) continue;
                var sec = (int) TimeParser
                    .ParseDuration(adv.Descendants("td").FirstOrDefault(x => x.HasClass("moveTime")).InnerText)
                    .TotalSeconds;
                var coordinates =
                    MapParser.GetCoordinates(adv.Descendants("td").FirstOrDefault(x => x.HasClass("coords")).InnerText);

                var difficulty = DifficultyEnum.Normal;
                switch (version)
                {
                    case ServerVersionEnum.T4_4:
                        difficulty = adv.Descendants("img").FirstOrDefault().GetAttributeValue("alt", "") == "Normal"
                            ? DifficultyEnum.Normal
                            : DifficultyEnum.Difficult;
                        break;
                    case ServerVersionEnum.T4_5:
                        difficulty =
                            adv.Descendants("img").FirstOrDefault().GetAttributeValue("class", "") ==
                            "adventureDifficulty1"
                                ? DifficultyEnum.Normal
                                : DifficultyEnum.Difficult;
                        break;
                }

                var secStr = adv.Descendants("td").FirstOrDefault(x => x.HasClass("timeLeft"))?.InnerText;
                var secRemaining = int.MaxValue;
                if (!string.IsNullOrEmpty(secStr)) secRemaining = (int) TimeParser.ParseDuration(secStr).TotalSeconds;

                switch (version)
                {
                    case ServerVersionEnum.T4_4:
                        var href = adv.Descendants("a").FirstOrDefault(x => x.HasClass("gotoAdventure"))
                            .GetAttributeValue("href", "").Replace("amp;", "");
                        adventuresList.Add(new Adventure
                        {
                            Coordinates = coordinates,
                            DurationSeconds = sec,
                            TimeLeftSeconds = secRemaining,
                            Difficulty = difficulty,
                            Ref = href
                        });
                        break;
                    case ServerVersionEnum.T4_5:
                        var elementId = adv.Descendants("td").FirstOrDefault(x => x.HasClass("goTo")).Id;
                        adventuresList.Add(new Adventure
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