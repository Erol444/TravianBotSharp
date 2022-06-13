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
            var adventures = htmlDoc.GetElementbyId("heroAdventure");
            if (adventures == null) return adventuresList;
            var tbody = adventures.Descendants("tbody").FirstOrDefault();
            if (tbody == null) return adventuresList;
            var trList = tbody.Descendants("tr");
            foreach (var adv in trList)
            {
                var tdList = adv.Descendants("td").ToArray();
                var sec = (int)TimeParser.ParseDuration(tdList[2].InnerText).TotalSeconds;
                var coordinates = MapParser.GetCoordinates(tdList[1].InnerText);

                var iconDifficulty = tdList[3].FirstChild;
                var difficulty = iconDifficulty.GetAttributeValue("alt", "").Contains("hard") ? DifficultyEnum.Difficult : DifficultyEnum.Normal; 
                
                        adventuresList.Add(new Adventure()
                        {
                            Coordinates = coordinates,
                            DurationSeconds = sec,
                            Difficulty = difficulty,
                        });
            }
            return adventuresList;
        }
    }
}