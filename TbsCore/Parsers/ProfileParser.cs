using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;
using TbsCore.Parsers;

namespace TbsCore.Parsers
{
    public static class ProfileParser
    {
        /// <summary>
        /// For NewYearSpecial servers where account's villages can be of different tribe
        /// </summary>
        /// <returns></returns>
        public static void ParseVillageTribes(Account acc, HtmlDocument html)
        {
            var tableBody = html.GetElementbyId("villages").ChildNodes.FindFirst("tbody");

            foreach (var th in tableBody.Descendants("tr"))
            {
                var kid = MapParser.GetKarteHref(th.Descendants("td").First(x => x.HasClass("name")));
                if (kid == null) continue;
                var coords = new Coordinates(acc, kid ?? 0);

                var vill = acc.Villages.First(v => v.Coordinates.Equals(coords));

                var tribeClass = th.Descendants("i")
                    .First(x => x.GetClasses().Any(y => y.StartsWith("tribe")))
                    .GetClasses()
                    .First();

                var tribe = (Classificator.TribeEnum)Parser.RemoveNonNumeric(tribeClass);

                vill.NysTribe = tribe;
            }
        }
    }
}