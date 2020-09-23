using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Files.Helpers
{
    public static class HeroHelper
    {
        /// <summary>
        /// Calculates if there is any adventure in the range of the home village.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>Whether there are adventures in range</returns>
        public static bool AdventureInRange(Account acc)
        {
            var heroHome = GetHeroHomeVillage(acc);
            if (heroHome == null) return false;

            return acc.Hero.Adventures.Any(x =>
                MapHelper.CalculateDistance(acc, x.Coordinates, heroHome.Coordinates) <= acc.Hero.Settings.MaxDistance
            );
        }

        /// <summary>
        /// Gets the hero home village
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>Hero home village</returns>
        public static Village GetHeroHomeVillage(Account acc)
        {
            return acc.Villages.FirstOrDefault(x => x.Id == acc.Hero.HomeVillageId);
        }

        /// <summary>
        /// Auto equip hero if there is better equipment available
        /// </summary>
        /// <param name="acc">Account</param>
        public static void AutoEquipHero(Account acc)
        {
            foreach (Classificator.HeroItemCategory category
                in (Classificator.HeroItemCategory[])Enum.GetValues(typeof(Classificator.HeroItemCategory)))
            {
                if (category == Classificator.HeroItemCategory.Others) continue; // Don't equip into hero bag
                int currentTier = 0;
                if(acc.Hero.Equipt.TryGetValue(category, out var item))
                {
                    // Hero already has an equipt item for this category
                    currentTier = GetHeroItemTier(item);
                }

                var equipWith = acc.Hero.Items
                    .Where(x => // Filter by category
                    {
                        (var itemCategory, var itemName, var itemTier) = ParseHeroItem(x.Item);
                        return itemCategory == category;
                    })
                    .OrderBy(x => // Order by tier
                    {
                        return GetHeroItemTier(x.Item);
                    })
                    .LastOrDefault();

                if(equipWith != null && GetHeroItemTier(equipWith.Item) > currentTier)
                {
                    TaskExecutor.AddTask(acc, new HeroEquip()
                    {
                        ExecuteAt = DateTime.Now,
                        Item = equipWith.Item
                    });
                }
            }
        }

        /// <summary>
        /// Will parse HeroItemEnum into category, name and tier
        /// </summary>
        /// <param name="item">Hero item enum</param>
        /// <returns>Hero item (category, name, tier)</returns>
        public static (Classificator.HeroItemCategory, string, int) ParseHeroItem(Classificator.HeroItemEnum item)
        {
            var attr = item.ToString().Split('_');

            Enum.TryParse(attr[0], out Classificator.HeroItemCategory category);
            string name = attr[1];
            int tier = int.Parse(attr[2]);

            return (category, name, tier);
        }

        /// <summary>
        /// Gets the tier of the hero item
        /// </summary>
        /// <param name="item">HeroItem</param>
        /// <returns>Tier</returns>
        public static int GetHeroItemTier(Classificator.HeroItemEnum item)
        {
            (var newCategory, var itemName, var itemTier) = ParseHeroItem(item);
            return itemTier;
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
            acc.Hero.HeroArrival = DateTime.Now + HeroParser.GetHeroArrivalInfo(acc.Wb.Html);

            UpdateHeroVillage(acc);
        }
        public static void UpdateHeroVillage(Account acc)
        {
            var hrefId = HeroParser.GetHeroVillageHref(acc.Wb.Html);
            if (hrefId == null) return;

            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    acc.Hero.HomeVillageId = hrefId ?? 0;
                    return;
                case Classificator.ServerVersionEnum.T4_5:
                    // Convert from coordinates id -> coordinates -> villageId
                    var coordinates = MapHelper.CoordinatesFromKid(hrefId ?? 0, acc);
                    var vill = acc.Villages.FirstOrDefault(x => x.Coordinates.Equals(coordinates));
                    if (vill == null) return;
                    acc.Hero.HomeVillageId = vill.Id;
                    return;
            }
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
