using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Models.Settings;
using TravBotSharp.Files.Models.TroopsModels;
using TravBotSharp.Files.Models.VillageModels;

namespace TravBotSharp.Files.Models.AccModels
{
    public class Village
    {
        public void Init(Account acc)
        {
            //When creating new village, init objects: coordinates, production and building array
            //Coordinates = new Coordinates();
            Troops = new VillTroops();
            Troops.Init(acc);
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
            Timings = new VillTiming();
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
        #endregion

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
        /// For village expansion (new village - settlers)
        /// </summary>
        public VillExpansion Expansion { get; set; }
        /// <summary>
        /// Timings for the village
        /// </summary>
        public VillTiming Timings { get; set; }
        #endregion
    }
}
