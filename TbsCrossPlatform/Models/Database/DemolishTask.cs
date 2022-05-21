using TbsCrossPlatform.Models.Enums;

namespace TbsCrossPlatform.Models.Database
{
    public class DemolishTask
    {
        public int VillageId { get; set; }
        public int Position { get; set; }
        public BuildingEnum Type { get; set; }
        public int Location { get; set; }
        public int Level { get; set; }
    }
}