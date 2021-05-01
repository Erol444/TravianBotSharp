using System.Collections.Generic;

namespace TbsCore.Models.VillageModels
{
    public class FarmingNonGold
    {
        public void Init()
        {
            ListFarm = new List<FarmList>();

            OasisFarmingDelay = 5; // 5 hours, should depend on the server speed
            MaxDeffPower = 3000; // Equates to about 30 rats + 15 snakes
            OasisMaxDistance = 10; // Max 10 squares away from the village
        }

        /// <summary>
        /// Non-goldclub farmlists
        /// </summary>
        public List<FarmList> ListFarm { get; set; }

        #region Oasis farmning
        /// <summary>
        /// Whether oasis farming enabled is enabled
        /// </summary>
        public bool OasisFarmingEnabled { get; set; }
        /// <summary>
        /// Minimum hours between attacking the same oasis again
        /// </summary>
        public int OasisFarmingDelay { get; set; }
        /// <summary>
        /// Maximum distance of the oasis that can be attacked by the bot
        /// </summary>
        public int OasisMaxDistance { get; set; }
        /// <summary>
        /// Maximum oasis deffensive power (infantry + cavalry combined) of the oasis
        /// that can be attacked by the bot
        /// </summary>
        public long MaxDeffPower { get; set; }
        /// <summary>
        /// Minimum offensive troops consumption in order to attack an oasis
        /// </summary>
        public long MinTroops { get; set; }
        /// <summary>
        /// Which oasis should be attacked first
        /// </summary>
        public OasisFarmingType OasisFarmingType { get; set; }
        #endregion Oasis farmning
    }

    public enum OasisFarmingType
    {
        /// <summary>
        /// Attack nearest oasis first
        /// </summary>
        NearestFirst,
        /// <summary>
        /// Attack oasis with least deffensive power first
        /// </summary>
        LeastPowerFirst,
        /// <summary>
        /// Attack oasis with most animals first
        /// </summary>
        MaxResFirst,
        /// <summary>
        /// Attack oasis for the best resource profit first (troops casualties vs resource plundered)
        /// </summary>
        MaxResProfitFirst, // Not yet implemented
    }
}