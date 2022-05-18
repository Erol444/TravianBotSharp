using static TbsCore.Helpers.Classificator;

namespace TbsCore.Models.SendTroopsModels
{
    /// <summary>
    /// Used when sending troops
    /// </summary>
    public class TroopsSendModel : TroopsMovementBase
    {
        /// <summary>
        /// Type of movement
        /// </summary>
        public MovementType MovementType { get; set; }

        /// <summary>
        /// Whether we want to redeploy the hero
        /// </summary>
        public bool RedeployHero { get; set; }

        /// <summary>
        /// Catapult targets 1 and 2
        /// </summary>
        public BuildingEnum Target1 { get; set; }

        public BuildingEnum Target2 { get; set; }

        /// <summary>
        /// Scouting type when spying
        /// </summary>
        public ScoutEnum ScoutType { get; set; }
    }

    public enum ScoutEnum
    {
        None = 0,
        Resources,
        Defences
    }
}