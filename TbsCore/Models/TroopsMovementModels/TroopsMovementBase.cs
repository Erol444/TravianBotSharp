using System;
using TbsCore.Models.MapModels;

namespace TbsCore.Models.SendTroopsModels
{
    public class TroopsMovementBase
    {
        /// <summary>
        /// Troops that are in movement. t1 - t11. In case we are under attack, will be 1 if "?" and 0 if "0",
        /// if we have spies artifact or number of attacking troops is less than rally point level.
        /// In case of sending out troops and a value is negative, it means send all available units
        /// </summary>
        public int[] Troops { get; set; }

        /// <summary>
        /// Target coordinates, where the troops are heading or currently are (reinforcements)
        /// </summary>
        public Coordinates TargetCoordinates { get; set; }

        /// <summary>
        /// When will troops reach their destination
        /// </summary>
        public DateTime Arrival { get; set; }
    }
}