using System;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.TroopsMovementModels
{
    /// <summary>
    ///     Troops movement parsed from dorf1
    /// </summary>
    public class TroopMovementDorf1
    {
        /// <summary>
        ///     Movement type
        /// </summary>
        public Classificator.MovementTypeDorf1 Type { get; set; }

        /// <summary>
        ///     Number of movements
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Time of the first/next arrival
        /// </summary>
        public DateTime Time { get; set; }
    }
}