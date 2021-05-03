using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.CombatModels
{
    public class CombatArmy : CombatArmyBase
    {

        public new void ApplyLosses(double losses)
        {
            base.ApplyLosses(losses);
            if ((Hero?.Info?.Health ?? 0) != 0)
            {
                var armBonus = Hero.GetArmorDmgReduction();
                Hero.Info.Health = (int)Math.Max(Hero.Info.Health - losses * 100.0F + armBonus, 0.0F);
            }
        }

        public new CombatPoints GetOffense()
        {
            var unitsOff = base.GetOffense();
            if ((Hero?.Info?.Health ?? 0) == 0) return unitsOff;

            var heroOff = Hero.GetOff();
            
            var (troop, boost) = Hero.GetWeaponBoost();
            var weaponBoost = CombatPoints.both(base.Troops[(int)troop % 10] * boost);

            return CombatPoints.sum(new CombatPoints[]{
                unitsOff, heroOff, weaponBoost
                }).Mul(Hero.GetOffBonus());
        }

        public new CombatPoints GetDeffense()
        {
            var unitsDeff = base.GetDeffense();
            if ((Hero?.Info?.Health ?? 0) == 0) return unitsDeff;

            var heroDeff = Hero.GetDeff();

            var (troop, boost) = Hero.GetWeaponBoost();
            var weaponBoost = CombatPoints.both(base.Troops[(int)troop % 10] * boost);

            return CombatPoints.sum(new CombatPoints[]{
                unitsDeff, heroDeff, weaponBoost
                }).Mul(Hero.GetDeffBonus());
        }
    }
}
