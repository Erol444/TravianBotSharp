using System;
using System.Collections.Generic;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;

namespace TbsCore.Models.VillageModels
{
    public class VillMarket
    {
        /// <summary>
        ///     Market settings
        /// </summary>
        public MarketSettings Settings { get; set; }

        /// <summary>
        ///     For NPC settings
        /// </summary>
        public NpcSettings Npc { get; set; }

        /// <summary>
        ///     List of all ongoing resource transits
        /// </summary>
        public List<MerchantsUnderWay> OngoingMerchants { get; set; }

        /// <summary>
        ///     Last transit of resources to this village
        /// </summary>
        public DateTime LastTransit { get; set; }

        public void Init()
        {
            Settings = new MarketSettings();
            Settings.Init();
            Npc = new NpcSettings();
            Npc.Init();
            OngoingMerchants = new List<MerchantsUnderWay>();
        }
    }
}