using System;
using System.Collections.Generic;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;

namespace TbsCore.Models.VillageModels
{
    public class VillMarket
    {
        public void Init()
        {
            AutoMarket = new AutoMarketSettings();
            AutoMarket.Init();
            Npc = new NpcSettings();
            Npc.Init();
            TradeRoute = new TradeRouteSettings();
            MerchantInfo = new Merchant();
            OngoingMerchants = new List<MerchantsUnderWay>();
        }

        /// <summary>
        /// For auto market settings
        /// </summary>
        public AutoMarketSettings AutoMarket { get; set; }

        /// <summary>
        /// For NPC settings
        /// </summary>
        public NpcSettings Npc { get; set; }

        /// <summary>
        /// For trade route
        /// </summary>
        public TradeRouteSettings TradeRoute { get; set; }

        /// <summary>
        /// List of all ongoing resource transits
        /// </summary>
        public List<MerchantsUnderWay> OngoingMerchants { get; set; }

        /// <summary>
        /// Merchant village's info
        /// </summary>
        public Merchant MerchantInfo { get; set; }

        /// <summary>
        /// Last transit of resources to this village
        /// </summary>
        public DateTime LastTransit { get; set; }
    }

    public class Merchant
    {
        /// <summary>
        /// Maximum merchant village has
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Current merchant in village
        /// </summary>
        public int Free { get; set; }

        /// <summary>
        /// Capacity of merchant
        /// </summary>
        public long Capacity { get; set; }

        /// <summary>
        /// Last update info time
        /// </summary>
        public DateTime LastUpdate { get; set; }
    }
}