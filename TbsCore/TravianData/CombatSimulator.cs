using TbsCore.Models.CombatModels;

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
            Combat combat = new Combat()
            {
                Attacker = attacker,
                Deffender = deffender
            };

            return raid ? combat.Raid() : combat.Normal();
        }
    }
}