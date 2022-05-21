using TbsCrossPlatform.Models.Enums;

namespace TbsCrossPlatform.Models.Database
{
    public class BuildingTask
    {
        public int VillageId { get; set; }
        public int Position { get; set; }

        /// <summary>
        /// Where should we build this building. If null, find a free space and set BuildingId to that.
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        /// Building to build
        /// </summary>
        public BuildingEnum Building { get; set; }

        /// <summary>
        /// To which level we want to upgrade the building
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// for warehouses, granaries, crannies, GG,GW. After construction, set it to false!
        /// </summary>
        public bool ConstructNew { get; set; }

        /// <summary>
        /// Which type of building task this is
        /// </summary>
        public BuildingType TaskType { get; set; }

        //for auto build res fields
        public ResTypeEnum ResourceType { get; set; }

        public BuildingStrategyEnum BuildingStrategy { get; set; }
    }
}