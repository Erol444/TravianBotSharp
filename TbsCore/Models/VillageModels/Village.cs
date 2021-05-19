using System.Collections.Generic;
using Newtonsoft.Json;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.Settings;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.VillageModels
{
    public class Village
    {
        public void Init(Account acc)
        {
            Troops = new VillTroops();
            Troops.Init();
            Res = new VillRes();
            Res.Init();
            Build = new VillBuilding();
            Build.Init(acc);
            Settings = new VillSettings();
            Settings.Init();
            Market = new VillMarket();
            Market.Init();
            Deffing = new VillDeffing();
            Expansion = new VillExpansion();
            FarmingNonGold = new FarmingNonGold();
            FarmingNonGold.Init();
            TroopMovements = new VillTroopMovements();
            TroopMovements.Init();
        }

        #region General info

        /// <summary>
        /// Id of the village
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the village
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is village under attack. Plus account usage
        /// TODO: move to vill.Deffing.UnderAttack
        /// </summary>
        public bool UnderAttack { get; set; }

        /// <summary>
        /// Coordinates of the village
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Is village currently selected
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Loyalty of the village, TODO implement
        /// </summary>
        public int Loyalty { get; set; }

        #endregion General info

        #region Specific areas

        /// <summary>
        /// Current resources, production, capacity etc.
        /// </summary>
        public VillRes Res { get; set; }

        /// <summary>
        /// Buildings, building tasks etc.
        /// </summary>
        public VillBuilding Build { get; set; }

        /// <summary>
        /// Market settings, NPC settings etc.
        /// </summary>
        public VillMarket Market { get; set; }

        /// <summary>
        /// Current troops, training settings etc.
        /// </summary>
        public VillTroops Troops { get; set; }

        /// <summary>
        /// General village settings, for Overview tab
        /// </summary>
        public VillSettings Settings { get; set; }

        /// <summary>
        /// For deffending settings, Deffing tab
        /// </summary>
        public VillDeffing Deffing { get; set; }

        /// <summary>
        /// Troops moving to/from the village
        /// </summary>
        public VillTroopMovements TroopMovements { get; set; }

        /// <summary>
        /// For village expansion (new village - settlers), culture points
        /// </summary>
        public VillExpansion Expansion { get; set; }

        /// <summary>
        /// Farm list for non gold
        /// </summary>
        public FarmingNonGold FarmingNonGold { get; set; }
        /// <summary>
        /// For NewYearSpecial servers where account's villages can be of different tribe
        /// </summary>
        public Classificator.TribeEnum NysTribe { get; internal set; }

        /// <summary>
        /// Tasks that weren't finished due to the lack of resources
        /// </summary>
        [JsonIgnore]
        public List<VillUnfinishedTask> UnfinishedTasks { get; set; }
        #endregion
    }
}