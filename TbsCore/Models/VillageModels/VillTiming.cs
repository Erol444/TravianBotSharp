using System;

namespace TbsCore.Models.VillageModels
{
    public class VillTiming
    {
        public void Init() { }
        /// <summary>
        /// When will be the next village refreshed (for resources only)
        /// </summary>
        public DateTime NextVillRefresh { get; set; }
    }
}
