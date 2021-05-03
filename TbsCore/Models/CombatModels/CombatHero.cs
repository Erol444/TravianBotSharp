using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.CombatModels
{
    public class CombatHero
    {
        /// <summary>
        /// Hero tribe. TODO: set it from the CombatArmy
        /// </summary>
        public TribeEnum Tribe { get; set; }

        /// <summary>
        /// Hero power, offensive / deffensive bonus
        /// </summary>
        public HeroInfo Info { get; set; }
        
        /// <summary>
        /// Hero items (natar horn, shield, right hand items)
        /// </summary>
        public Dictionary<HeroItemCategory, HeroItemEnum> Items { get; set; }


        public CombatPoints GetOff() => CombatPoints.off(GetTotalStrength(), HasHorse());
        public CombatPoints GetDeff()
        {
            var str = GetTotalStrength();
            return new CombatPoints(str, str);
        }

        // TODO: take into account natars horn
        public double GetOffBonus() => 1.0F + 0.2F * Info?.OffBonusPoints ?? 0;

        public double GetDeffBonus() => 1.0F + 0.2F * Info?.DeffBonusPoints ?? 0;

        private int GetTotalStrength() => GetStrength() + GetItemsStrength();
        private int GetStrength()
        {
            if (Info == null) return 0;
            int power = 100; // Base hero power
            var levelMultiplier = Tribe == TribeEnum.Romans ? 100 : 80;
            power += levelMultiplier * Info.FightingStrengthPoints;
            return power;
        }

        public int GetArmorDmgReduction()
        {
            if (Items.TryGetValue(HeroItemCategory.Armor, out var armor))
            {
                var (_, name, tier) = HeroHelper.ParseHeroItem(armor);
                return HeroHelper.GetArmorDmgReduce(name, tier);
            }
            return 0;
        }

        private int GetItemsStrength()
        {
            if (Info == null || Items == null) return 0;

            int power = 0;
            if (Items.TryGetValue(HeroItemCategory.Weapon, out var weapon))
            {
                var (_, _, tier) = HeroHelper.ParseHeroItem(weapon);
                power += tier * 500;
            }
            if (Items.TryGetValue(HeroItemCategory.Armor, out var armor))
            {
                var (_, name, tier) = HeroHelper.ParseHeroItem(armor);
                power += HeroHelper.GetArmorStrength(name) * tier;
            }
            if (Items.TryGetValue(HeroItemCategory.Left, out var left))
            {
                var (_, name, tier) = HeroHelper.ParseHeroItem(left);
                if (name == "Shield") power += tier * 500;
            }
            return power;
        }

        /// <summary>
        /// Whether hero has horse equipped
        /// </summary>
        private bool HasHorse() => Items?.TryGetValue(HeroItemCategory.Horse, out _) ?? false;

        public (TroopsEnum, int) GetWeaponBoost()
        {
            if (Items == null) return (TroopsEnum.None, 0);
            if (Items.TryGetValue(HeroItemCategory.Weapon, out var weapon))
            {
                var (troop, boost) = HeroHelper.ParseWeapon(weapon);
                if ((int)this.Tribe - 1 == (int)troop / 10)
                {
                    // If we have hero weapon for our tribe
                    return (troop, boost);
                }
            }
            return (TroopsEnum.None, 0);
        }
    }
}