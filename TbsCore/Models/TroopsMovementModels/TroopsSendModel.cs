using System;
using TbsCore.Models.MapModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.SendTroopsModels
{
    /// <summary>
    /// Used when sending troops
    /// </summary>
    public class TroopsSendModel : TroopsMovementModel
    {
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