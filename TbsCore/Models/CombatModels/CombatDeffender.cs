using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.CombatModels
{
    public class CombatDeffender
    {
        /// <summary>
        /// List of (Troops, hero, tribe), since deffender can have armies from multiple accounts / villages
        /// </summary>
        public List<CombatBase> Armies { get; set; }
        
        /// <summary>
        /// Population of the deffender, morale bonus depends on it
        /// </summary>
        public int Population { get; set; }
        
        /// <summary>
        /// Level of the wall inside the village
        /// </summary>
        public int WallLevel { get; set; }
        
        /// <summary>
        /// Which tribe is the deffender. Wall bonus depends on the tribe
        /// </summary>
        public Classificator.TribeEnum DeffTribe { get; set; }

        /// <summary>
        /// Ally metalurgy percentage (2% => 2, 4% => 4)
        /// </summary>
        public int Metalurgy { get; set; }
    }
}
