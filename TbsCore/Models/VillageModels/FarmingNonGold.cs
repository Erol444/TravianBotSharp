using System.Collections.Generic;

namespace TbsCore.Models.VillageModels
{
    public class FarmingNonGold
    {
        public void Init()
        {
            ListFarm = new List<FarmList>();
        }

        public List<FarmList> ListFarm { get; set; }
    }
}