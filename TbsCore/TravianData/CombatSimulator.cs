using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.CombatModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.TravianData;

namespace TbsCore.TravianData
{
    // https://wbb.forum.travian.com/index.php?thread/75248-combat-system-formulas/
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
            // Basic village defense
            deffense += 10;
            var (wallBonus, wallDeff) = BuildingsData.GetWallBonus(deffender.DeffTribe, deffender.WallLevel);
            deffense += wallDeff;
            deffense = (int)Math.Round(deffense * wallBonus, MidpointRounding.AwayFromZero);

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
        public static int GetRealDeffense(CombatAttacker attacker, CombatDeffender deffender)
        {
            var offense = GetArmyOffense(attacker.Army);
            var deffense = GetArmyDeffense(deffender.Armies);            

            return GetRealDeffense(offense, deffense);
        }
        public static int GetRealDeffense((int, int) offense, (int, int) deffense)
        {
            var (offInf, offCav) = offense;
            var (deffInf, deffCav) = deffense;

            float ratio = offInf / (float)(offCav + offInf);

            var normalizedInf = (float)deffInf * ratio;
            var normalizedCav = (float)deffCav * (1 - ratio);
            var rounded = (int)Math.Round((float)(normalizedInf + normalizedCav), MidpointRounding.AwayFromZero);
            return rounded;
        }

        /// <summary>
        /// Offensive power is just sum of (inf + cav)
        /// </summary>
        public static int GetRealOffense(CombatAttacker attacker) =>
            GetRealOffense(attacker.Army);
        public static int GetRealOffense(CombatBase army)
        {
            var (inf, cav) = GetArmyOffense(army);
            return inf + cav;
        }


        private static (int, int) GetArmyOffense(CombatBase army)
        {
            int inf = 0, cav = 0;
            for (int i = 0; i < 10; i++)
            {
                if (army.Troops[i] == 0) continue;
                var troop = TroopsHelper.TroopFromInt(army.Tribe, i);
                var off = TroopsData.GetTroopOff(troop);

                if (TroopsData.IsInfantry(troop)) inf += off * army.Troops[i];
                else cav += off * army.Troops[i];
            }
            return (inf, cav);
        }

        private static (int, int) GetArmyDeffense(List<CombatBase> armies)
        {
            int inf = 0, cav = 0;
            foreach(var army in armies)
            {
                var (infDeff, cavDeff) = GetArmyDeffense(army);
                inf += infDeff;
                cav += cavDeff;
            }
            return (inf, cav);
        }

        private static (int, int) GetArmyDeffense(CombatBase army)
        {
            int inf = 0, cav = 0;
            for (int i = 0; i < 10; i++)
            {
                if (army.Troops[i] == 0) continue;
                var troop = TroopsHelper.TroopFromInt(army.Tribe, i);
                var (deffInf, deffCav) = TroopsData.GetTroopDeff(troop);
                inf += deffInf * army.Troops[i];
                cav += deffCav * army.Troops[i];
            }
            return (inf, cav);
        }
    }
}