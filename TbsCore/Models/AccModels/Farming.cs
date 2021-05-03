using System;
using System.Collections.Generic;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;

namespace TbsCore.Models.AccModels
{
    public class Farming
    {
        public Farming()
        {
            FL = new List<FarmList>();
            OasisFarmed = new List<(Coordinates, DateTime)>();
        }
        public int MinInterval { get; set; }
        public int MaxInterval { get; set; }
        public bool Enabled { get; set; }

        //More for high speed TTWARS servers
        public bool TrainTroopsAfterFL { get; set; }

        public List<FarmList> FL { get; set; }
        //add if attack even with loses...
        //multiple FL support!

        /// <summary>
        /// When was the oasis last attacked
        /// </summary>
        public List<(Coordinates, DateTime)> OasisFarmed { get; set; }
    }
}
