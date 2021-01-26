using System.Collections.Generic;

namespace TbsCore.Models.FarmingNonGoldModels
{
    public class Framing
    {
        public Framing()
        {
            ListFarm = new List<FarmList>();
        }

        public List<FarmList> ListFarm { get; set; }
    }
}