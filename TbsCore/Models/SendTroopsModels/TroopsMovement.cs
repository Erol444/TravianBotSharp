using System;
using TbsCore.Models.MapModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.SendTroopsModels
{
    public class TroopsMovement
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
        /// <summary>
        /// Whether we want to redeploy hero
        /// </summary>
        public bool RedeployHero { get; set; }

        /// <summary>
        /// Catapult targets
        /// </summary>
        public BuildingEnum Target1 { get; set; }
        public BuildingEnum Target2 { get; set; }
        public ScoutEnum ScoutType { get; set; }
    }

    public enum ScoutEnum
    {
        None = 0,
        Resources,
        Defences
    }
}
