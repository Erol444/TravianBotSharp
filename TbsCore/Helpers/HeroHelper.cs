using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Helpers
{
    public static class HeroHelper
    {
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
        /// Will parse all the useful data from the hero page (/hero.php)
        /// </summary>
        /// <param name="acc">Account</param>
        public static void ParseHeroPage(Account acc)
        {
            acc.Settings.Timing.LastHeroRefresh = DateTime.Now;
            acc.Hero.HeroInfo = HeroParser.GetHeroInfo(acc.Wb.Html);
            acc.Hero.Items = HeroParser.GetHeroItems(acc.Wb.Html);
            acc.Hero.Equipt = HeroParser.GetHeroEquipment(acc.Wb.Html);

            var homeVill = HeroParser.GetHeroVillageId(acc.Wb.Html);
            if (homeVill != null) acc.Hero.HomeVillageId = homeVill ?? 0;
        }

        public static Resources GetHeroResources(Account acc)
        {

            var heroItems = acc.Hero.Items;
            var res = new Resources()
            {
                Wood = heroItems.FirstOrDefault(x => x.Item == Classificator.HeroItemEnum.Others_Wood_0)?.Count ?? 0,
                Clay = heroItems.FirstOrDefault(x => x.Item == Classificator.HeroItemEnum.Others_Clay_0)?.Count ?? 0,
                Iron = heroItems.FirstOrDefault(x => x.Item == Classificator.HeroItemEnum.Others_Iron_0)?.Count ?? 0,
                Crop = heroItems.FirstOrDefault(x => x.Item == Classificator.HeroItemEnum.Others_Crop_0)?.Count ?? 0
            };

            return res;
        }
    }
}
