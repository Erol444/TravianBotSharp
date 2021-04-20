using System;

namespace TbsCore.Models.VillageModels
{
    public class VillTiming
    {
        /// <summary>
        ///     When will be the next village refreshed (for resources only)
        /// </summary>
        public DateTime NextVillRefresh { get; set; }

        public void Init()
        {
        }
    }
}