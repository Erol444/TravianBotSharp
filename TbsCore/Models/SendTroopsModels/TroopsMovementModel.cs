using System;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Models.SendTroopsModels
{
    public class TroopsMovementModel
    {
        /// <summary>
        /// Type of movement (raid/attack/reinforcement)
        /// </summary>
        public MovementType MovementType { get; set; }
        /// <summary>
        /// When will troops reach their destination
        /// </summary>
        public DateTime Arrival { get; set; }
        /// <summary>
        /// Troops that are in movement. t1 - t11. In case we are under attack, will be 1 if "?" and 0 if "0",
        /// if we have spies artifact or number of attacking troops is less than rally point level
        /// </summary>
        public int[] Troops { get; set; }
        /// <summary>
        /// Target village (the one we are attacking or is attacking us)
        /// </summary>
        public Coordinates Coordinates { get; set; }
    }
}
