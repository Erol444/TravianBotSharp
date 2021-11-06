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
        /// Last transit of resources to this village
        /// </summary>
        public DateTime LastTransit { get; set; }
    }
}