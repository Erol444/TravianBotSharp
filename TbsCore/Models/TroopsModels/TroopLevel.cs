using System;
using TbsCore.Models.ResourceModels;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.TroopsModels
{
    /// <summary>
    /// For troops upgraded in smithy. Based on this we can also know which troops have already been researched
    /// </summary>
    public class TroopLevel
    {
        public Classificator.TroopsEnum Troop { get; set; }
        public int Level { get; set; }
        public Resources UpgradeCost { get; set; }
        /// <summary>
        /// Time it would take for this troop to upgrade in smithy
        /// </summary>
        public TimeSpan TimeCost { get; set; }
    }
}
