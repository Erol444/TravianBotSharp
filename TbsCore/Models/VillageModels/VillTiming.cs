using System;
using System.Collections.Generic;
using System.Text;

namespace TbsCore.Models.VillageModels
{
    public class VillTiming
    {
        public void Init() { }
        /// <summary>
        /// When was the village last refreshed (for resources only)
        /// </summary>
        public DateTime LastVillRefresh { get; set; }
    }
}
