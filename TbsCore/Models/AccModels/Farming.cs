using System.Collections.Generic;
using TbsCore.Models.TroopsModels;

namespace TbsCore.Models.AccModels
{
    public class Farming
    {
        public Farming()
        {
            FL = new List<FarmList>();
        }
        public int MinInterval { get; set; }
        public int MaxInterval { get; set; }
        public bool Enabled { get; set; }

        //More for high speed TTWARS servers
        public bool TrainTroopsAfterFL { get; set; }

        public List<FarmList> FL { get; set; }
        //add if attack even with loses...
        //multiple FL support!
    }
}
