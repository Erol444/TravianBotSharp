using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.TravianData;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.CombatModels
{
    public class Combat
    {
        public CombatAttacker Attacker { get; set; }
        public CombatDeffender Deffender { get; set; }


        private (long, long) finalState;
        private (long, long) baseState;

        private readonly int baseVillDeff = 10;

        public (double, double) Raid()
        {
            CalculateTotalPoints();
            var ratio = CalculateRatio();
            // Calc losses
            return RaidRatio(ratio);
        }

        public (double, double) Normal()
        {
            CalculateTotalPoints();
            // CalculateRams()
            var ratio = CalculateRatio();
            return NormalRatio(ratio);
            // CalculateCatapults()
        }

        private (double, double) RaidRatio(double x) => (1 / (1 + x), x / (1 + x));
        private (double, double) NormalRatio(double x) => (Math.Min(1 / x, 1), Math.Min(x, 1));

        private double CalculateBaseRatio() => finalState.Item1 / (double)finalState.Item2;
        private double CalculateRatio() => Math.Pow(CalculateBaseRatio(), GetImmensityFactor());

        /// <summary>
        /// Get Morale/Population bonus. Public for testing purposes
        /// </summary>
        public double GetMoraleBonus(double ptsRatio = 1)
        {
            if (Deffender.DeffTribe == TribeEnum.Nature)
            {
                // Note: Nature, the 'account' for unoccupied oases, has its own population (500).
                Deffender.Population = 500;
            }

            if (Attacker.Population < Deffender.Population) return 1.0F;

            // So we don't divide by 0
            if (Deffender.Population == 0) return 1.5F;
            if (Attacker.Population == 0) return 1.0F;

            double popRatio = Attacker.Population / (double)Math.Max(Deffender.Population, 3);
            return Math.Max(0.667, Math.Pow(popRatio, -0.2F * Math.Min(ptsRatio, 1)));
        }

        private double GetWallBonus() // getDefBonus
        {
            var (bonus, _) = BuildingsData.GetWallBonus(Deffender.DeffTribe, Deffender.WallLevel);
            return bonus;
        }

        private double GetCataMorale()
        {
            // limit(0.3333, 1)((offPop / defPop) ** -0.3);
            if (Deffender.Population == 0) return 1;
            var morale = Math.Pow(Attacker.Population / Deffender.Population, -0.3F);
            if (morale < 0.3333F) return 0.3333F;
            if (1.0F < morale) return 1.0F;
            return morale;
        }

        private int GetVillDeff() // getDefAbsolute
        {
            var (_, wallDeff) = BuildingsData.GetWallBonus(Deffender.DeffTribe, Deffender.WallLevel);
            var palaceDeff = 2 * Math.Pow(Deffender.PalaceLevel, 2);
            return wallDeff + baseVillDeff + (int)palaceDeff;
        }

        /// <summary>
        /// Public for testing purposes
        /// </summary>
        public (long, long) CalculateBaseState() // calcBasePoints
        {
            var offPts = this.Attacker.Army.GetOffense();
            var defPts = Deffender.GetDeffense();
            var (off, def) = GetAducedDef(offPts, defPts);
            baseState = ( off, def );
            return baseState;
        }

        private (long, long) CalculateTotalPoints()
        {
            var (baseOff, baseDeff) = CalculateBaseState();
            var finalDef = RoundLong((baseDeff + GetVillDeff()) * GetWallBonus());
            var morale = GetMoraleBonus(baseOff / finalDef);
            var finalOff = RoundLong(baseOff * morale);
            finalState = (finalOff, finalDef);
            return finalState;
        }

        // K depends on how much soldiers were involved in combat. Really, large, immense battles 
        // should differ from small battles between hundreds of soldiers.
        private double GetImmensityFactor()
        {
            // N is total amount of units taking part in battle (unit count, not their wheat upkeep)
            long N = 0;
            N += Attacker.Army.GetTotal();
            Deffender.Armies.ForEach(army => N += army.GetTotal());

            // K = 2 · (1.8592 – N^0.015)
            double K = 2 * (1.8592 - Math.Pow(N, 0.015));
            if (1.5 < K) return 1.5;
            if (K < 1.2578) return 1.2578;
            return K;
        }

        private (long, long) GetAducedDef(CombatPoints off, CombatPoints deff)
        {
            var totalOff = off.i + off.c;
            var infantryPart = RoundPercent(off.i / (double)totalOff);
            var cavalryPart = RoundPercent(off.c / (double)totalOff);
            var totalDef = deff.i * infantryPart + deff.c * cavalryPart;
            return (totalOff, RoundLong(totalDef));
        }

        private double RoundPercent(double val) =>
            Math.Round(val, 4, MidpointRounding.AwayFromZero);
        private long RoundLong(double val) =>
            (long)Math.Round(val, MidpointRounding.AwayFromZero);
    }
}
