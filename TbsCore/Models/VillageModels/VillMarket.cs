using System.Collections.Generic;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Models.Settings;

namespace TravBotSharp.Files.Models.VillageModels
{
    public class VillMarket
    {
        public void Init()
        {
            Settings = new MarketSettings();
            Settings.Init();
            Npc = new NpcSettings();
            Npc.Init();
            OngoingMerchants = new List<MerchantsUnderWay>();
        }

        /// <summary>
        /// Market settings
        /// </summary>
        public MarketSettings Settings { get; set; }
        /// <summary>
        /// For NPC settings
        /// </summary>
        public NpcSettings Npc { get; set; }
        /// <summary>
        /// List of all ongoing resource transits
        /// </summary>
        public List<MerchantsUnderWay> OngoingMerchants { get; set; }
    }
}
