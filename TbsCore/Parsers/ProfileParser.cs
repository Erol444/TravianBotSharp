using System.Linq;
using HtmlAgilityPack;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TbsCore.Parsers
{
    public static class ProfileParser
    {
        /// <summary>
        ///     For NewYearSpecial servers where account's villages can be of different tribe
        /// </summary>
        /// <returns></returns>
        public static void ParseVillageTribes(Account acc, HtmlDocument html)
        {
            var tableBody = html.GetElementbyId("villages").ChildNodes.FindFirst("tbody");

            foreach (var th in tableBody.Descendants("tr"))
            {
                var kid = MapParser.GetKarteHref(th.Descendants("td").First(x => x.HasClass("name")));
                if (kid == null) continue;
                var coords = MapHelper.CoordinatesFromKid(kid ?? 0, acc);

                var vill = acc.Villages.First(v => v.Coordinates.Equals(coords));

                var tribeClass = th.Descendants("i")
                    .First(x => x.GetClasses().Any(y => y.StartsWith("tribe")))
                    .GetClasses()
                    .First();

                var tribe = (Classificator.TribeEnum) Parser.RemoveNonNumeric(tribeClass);

                vill.NysTribe = tribe;
            }
        }
    }
}