using System;
using TbsCrossPlatform.Models.Enums;

namespace TbsCrossPlatform.Models
{
    public class BuildingCurrently
    {
        public int VillageId { get; set; }
        public int Location { get; set; }
        public BuildingEnum Type { get; set; }
        public int Level { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}