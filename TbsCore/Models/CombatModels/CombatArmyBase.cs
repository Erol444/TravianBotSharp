using System;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.Models.TroopsModels;
using TbsCore.TravianData;

namespace TbsCore.Models.CombatModels
{
    public class CombatArmyBase : TroopsBase
    {
        public int[] Improvements { get; set; }
        public CombatHero Hero { get; set; }

        /// <summary>
        /// For testing purposes only
        /// </summary>
        public bool IgnoreImprovements { get; set; }


        public void ApplyLosses(double losses)
        {
            for (int i = 0; i < this.Troops.Length; i++)
            {
                this.Troops[i] = (int)Math.Round(this.Troops[i] * (1 - losses), MidpointRounding.AwayFromZero);
            }
        }
        public int GetTotal() => Troops.Sum();

        public CombatPoints GetOffense()
        {
            double inf = 0, cav = 0;
            for (int i = 0; i < 10; i++)
            {
                if (Troops[i] == 0) continue;
                var troop = TroopsHelper.TroopFromInt(Tribe, i);

                var lvl = Improvements == null || IgnoreImprovements ? 1 : Improvements[i];
                var off = TroopsData.GetTroopOff(troop, lvl);

                if (TroopsData.IsInfantry(troop)) inf += off * Troops[i];
                else cav += off * Troops[i];
            }
            return new CombatPoints(inf, cav);
        }

        public CombatPoints GetDeffense()
        {
            double inf = 0, cav = 0;
            for (int i = 0; i < 10; i++)
            {
                if (Troops[i] == 0) continue;
                var troop = TroopsHelper.TroopFromInt(Tribe, i);
                var lvl = Improvements == null || IgnoreImprovements ? 1 : Improvements[i];
                var (deffInf, deffCav) = TroopsData.GetTroopDeff(troop, lvl);

                inf += deffInf * Troops[i];
                cav += deffCav * Troops[i];
            }
            return new CombatPoints(inf, cav);
        }

        /// <summary>
        /// Gets rams number and their upgrade level
        /// </summary>
        public (int, int) GetRams() => (Troops[7], Improvements[7]);

        /// <summary>
        /// Gets rams number and their upgrade level
        /// </summary>
        public (int, int) GetCatapults() => (Troops[8], Improvements[8]);

    }
}
