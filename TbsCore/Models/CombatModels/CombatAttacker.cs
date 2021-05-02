using System;
using System.Collections.Generic;
using System.Text;

namespace TbsCore.Models.CombatModels
{
    public class CombatAttacker
    {
        /// <summary>
        /// Troops, hero, tribe
        /// </summary>
        public CombatBase Army { get; set; }
        
        /// <summary>
        /// Population of the attacker, morale bonus depends on it
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// Ally metalurgy percentage (2% => 2, 4% => 4)
        /// </summary>
        public int Metalurgy { get; set; }
    }
}
