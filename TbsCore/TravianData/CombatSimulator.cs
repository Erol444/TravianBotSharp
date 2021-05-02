using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.CombatModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.TravianData;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.TravianData
{
    /// <summary>
    /// Source:
    /// https://wbb.forum.travian.com/index.php?thread/75248-combat-system-formulas/ 
    /// https://github.com/kirilloid/travian/tree/master/src/model/base/combat
    /// TODO: take into the account destroying wall with rams, hero bonus, metalurgy,
    /// hero items, palace deff
    /// </summary>
    public static class CombatSimulator
    {
        /// <summary>
        /// Gets casualties ratio
        /// </summary>
        /// <returns>(attacker casualties, deffender casualties)</returns>
        public static (double, double) GetCasualties(CombatAttacker attacker, CombatDeffender deffender, bool raid)
        {
            var offense = GetRealOffense(attacker);
            var deffense = GetRealDeffense(attacker, deffender);
            
            // Take into the account wall bonus / deffense
            var (wallBonus, wallDeff) = BuildingsData.GetWallBonus(deffender.DeffTribe, deffender.WallLevel);
            
            // Take into the account morale (pop) bonus
            var subDeffense = (int)Math.Round(deffense * wallBonus, MidpointRounding.AwayFromZero);
            subDeffense += 10 + wallDeff;
            var popBonus = GetPopBonus(attacker, deffender, offense, subDeffense);

            var totalBonus = wallBonus * popBonus;
            deffense = (int)Math.Round(deffense * totalBonus, MidpointRounding.AwayFromZero);

            // Basic village defense
            deffense += 10;
            deffense += wallDeff;
            // TODO: add palace deff

            double ratio = (float)offense / deffense;
            bool attackerWon = true;
            var K = GetK(attacker, deffender);
            if (1 < ratio)
            {
                ratio = Math.Pow(((double)deffense / offense), K);
            }
            else
            {
                attackerWon = false;
                ratio = Math.Pow(((double)offense / deffense), K);
            }
            
            // If normal attack, the one who lost has 100% casualties and the winner {ratio}% 
            if (raid == false)
            {
                if (attackerWon) return (ratio, 1.0);
                return (1.0, ratio);
            }

            var raidRatio = RaidRatio(ratio);
            
            if (attackerWon) return (raidRatio, 1 - raidRatio);
            return (1 - raidRatio, raidRatio);
        }

        #region Private methods
        /// <summary>
        /// Get Morale/Population bonus
        /// </summary>
        private static double GetPopBonus(CombatAttacker attacker, CombatDeffender deffender, double off, double deff)
        {
            if (deffender.DeffTribe == TribeEnum.Nature) 
            {
                // Note: Nature, the 'account' for unoccupied oases, has its own population (500).
                deffender.Population = 500;
            }

            if (attacker.Population < deffender.Population) return 1.0F;
            
            // So we don't divide by 0
            if (deffender.Population == 0) return 1.5F;
            if (attacker.Population == 0) return 1.0F;

            double bonus;
            if (deff < off)
            {
                // M^0.2, where M is attacker's population / defender's population
                bonus = Math.Pow((attacker.Population / (double)deffender.Population), 0.2F);
            }
            else
            {
                // If the attacker has fewer points than defender:
                // M ^{ 0.2·(offense points / defense points)}
                double exp = 0.2F * (off / (double)deff);
                bonus = Math.Pow((attacker.Population / (double)deffender.Population), exp);
            }

            // Moralebonus never goes higher than +50%
            if (1.5F < bonus) bonus = 1.5;

            return bonus;
        }

        // For raids, the formula changes a bit, losses will be: 100% · x / (100% + x)
        private static double RaidRatio(double ratio) =>
            ratio / (1 + ratio);

        // K depends on how much soldiers were involved in combat. Really, large, immense battles 
        // should differ from small battles between hundreds of soldiers.
        private static double GetK(CombatAttacker attacker, CombatDeffender deffender)
        {
            // N is total amount of units taking part in battle (unit count, not their wheat upkeep)
            long N = 0;
            N += GetNumTroops(attacker.Army);
            deffender.Armies.ForEach(army => N += GetNumTroops(army));

            // K = 2 · (1.8592 – N^0.015)
            double K = 2 * (1.8592 - Math.Pow(N, 0.015));
            if (1.5 < K) return 1.5;
            if (K < 1.2578) return 1.2578;
            return K;
        }
        private static int GetNumTroops(CombatBase army) => army.Troops.Sum();

        /// <summary>
        /// Gets the "real" (normalized) deffensive power of the deffender. Used by the combat simulator.
        /// </summary>
        private static double GetRealDeffense(CombatAttacker attacker, CombatDeffender deffender)
        {
            var offense = GetArmyOffense(attacker.Army);
            var deffense = GetArmyDeffense(deffender.Armies);            

            return GetRealDeffense(offense, deffense);
        }
        public static int GetRealDeffense((double, double) offense, (double, double) deffense)
        {
            var (offInf, offCav) = offense;
            var (deffInf, deffCav) = deffense;

            double ratio = offInf / (offCav + offInf);

            var normalizedInf = (float)deffInf * ratio;
            var normalizedCav = (float)deffCav * (1 - ratio);
            var rounded = (int)Math.Round((float)(normalizedInf + normalizedCav), MidpointRounding.AwayFromZero);
            return rounded;
        }

        /// <summary>
        /// Offensive power is just sum of (inf + cav)
        /// </summary>
        public static double GetRealOffense(CombatAttacker attacker) =>
            GetRealOffense(attacker.Army);
        public static double GetRealOffense(CombatBase army)
        {
            var (inf, cav) = GetArmyOffense(army);
            return (inf + cav);
        }

        private static (double, double) GetArmyDeffense(List<CombatBase> armies)
        {
            double inf = 0, cav = 0;
            foreach(var army in armies)
            {
                var (infDeff, cavDeff) = GetArmyDeffense(army);
                inf += infDeff;
                cav += cavDeff;
            }
            return (inf, cav);
        }
        private static (double, double) GetArmyOffense(CombatBase army)
        {
            double inf = 0, cav = 0;
            for (int i = 0; i < 10; i++)
            {
                if (army.Troops[i] == 0) continue;
                var troop = TroopsHelper.TroopFromInt(army.Tribe, i);
                
                var lvl = army.Improvements == null ? 1 : army.Improvements[i];
                var off = TroopsData.GetTroopOff(troop, lvl);
                
                if (troop == troopBoost) off += boost;
                
                if (TroopsData.IsInfantry(troop)) inf += off * army.Troops[i];
                else cav += off * army.Troops[i];
            }
            return (inf, cav);
        }

        private static (double, double) GetArmyDeffense(CombatBase army)
        {
            var (troopBoost, boost) = GetWeaponBoost(army.Hero);

            double inf = 0, cav = 0;
            for (int i = 0; i < 10; i++)
            {
                if (army.Troops[i] == 0) continue;
                var troop = TroopsHelper.TroopFromInt(army.Tribe, i);
                var lvl = army.Improvements == null ? 1 : army.Improvements[i];
                var (deffInf, deffCav) = TroopsData.GetTroopDeff(troop, lvl);

                if(troop == troopBoost)
                {
                    inf += boost;
                    cav += boost;
                }

                inf += deffInf * army.Troops[i];
                cav += deffCav * army.Troops[i];
            }
            return (inf, cav);
        }

        private static (TroopsEnum, int) GetWeaponBoost(CombatHero hero)
        {
            if (hero == null) return (TroopsEnum.None, 0);
            if (hero.Items.TryGetValue(HeroItemCategory.Weapon, out var weapon))
            {
                return HeroHelper.ParseWeapon(weapon);
            }
            return (TroopsEnum.None, 0);
        }

        /// <summary>
        /// Gets hero bonus, power and whether it's on the horse
        /// </summary>
        /// <returns>(bonus, power)</returns>
        private static (double, int, bool) GetHeroBoost(CombatBase army, bool attack)
        {
            var hero = army.Hero;
            if (hero == null || hero.Info == null || hero.Items == null) return (1.0, 0, false);

            int power = 100; // Base hero power
            if (hero.Items.TryGetValue(HeroItemCategory.Weapon, out var weapon))
            {
                var (_, _, tier) = HeroHelper.ParseHeroItem(weapon);
                power += tier * 500;
            }
            if (hero.Items.TryGetValue(HeroItemCategory.Armor, out var armor))
            {
                var (_, name, tier) = HeroHelper.ParseHeroItem(armor);
                power += HeroHelper.GetArmorBaseStrength(name) * tier;
            }
            if (hero.Items.TryGetValue(HeroItemCategory.Left, out var left))
            {
                var (_, name, tier) = HeroHelper.ParseHeroItem(left);
                if (name == "Shield") power += tier * 500;
            }
            var levelMultiplier = army.Tribe == TribeEnum.Romans ? 100 : 80;
            power += levelMultiplier * hero.Info.FightingStrengthPoints;

            double bonus = 0.2F * (attack ? hero.Info.OffBonusPoints : hero.Info.DeffBonusPoints);
            bonus += 1.0F;

            bool horse = hero.Items.TryGetValue(HeroItemCategory.Horse, out _);

            return (bonus, power, horse);
        }
        #endregion Private methods
    }
}