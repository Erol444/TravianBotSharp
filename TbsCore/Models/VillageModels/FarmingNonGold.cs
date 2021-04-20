using System.Collections.Generic;

namespace TbsCore.Models.VillageModels
{
    public class FarmingNonGold
    {
        public List<FarmList> ListFarm { get; set; }

        public void Init()
        {
            ListFarm = new List<FarmList>();
        }
    }
}