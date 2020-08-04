using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Helpers
{
    public static class HeroHelper
    {
        private static readonly Dictionary<int, string> DomId = new Dictionary<int, string>()
        {
            { 0, "attributepower" },
            { 0, "attributeoffBonus" },
            { 0, "attributedefBonus" },
            { 0, "attributeproductionPoints" },
        };
        /// <summary>
        /// Calculates if there is any adventure in the range of the home village.
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool AdventureInRange(Account acc)
        {
            return acc.Hero.Adventures.Any(x =>
                MapHelper.CalculateDistance(acc, x.Coordinates, MapHelper.CoordinatesFromKid(acc.Hero.HomeVillageId, acc)) <= acc.Hero.Settings.MaxDistance
            );
        }

        /// <summary>
        /// Gets the html DOM id from the iterator
        /// </summary>
        /// <param name="i">Iterator</param>
        /// <returns></returns>
        public static string AttributeDomId(int i)
        {
            DomId.TryGetValue(i, out string ret);
            return ret;
        }
    }
}
