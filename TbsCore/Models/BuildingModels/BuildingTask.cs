using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks;
using static TravBotSharp.Files.Helpers.BuildingHelper;

namespace TbsCore.Models.BuildingModels
{
    public class BuildingTask
    {
        /// <summary>
        /// Where should we build this building. If null, find a free space and set BuildingId to that.
        /// </summary>
        public byte? BuildingId { get; set; }
        /// <summary>
        /// Building to build
        /// </summary>
        public Classificator.BuildingEnum Building { get; set; }
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
        public Classificator.BuildingType TaskType { get; set; }

        //for auto build res fields
        public ResTypeEnum ResourceType { get; set; }
        public BuildingStrategyEnum BuildingStrategy { get; set; }
    }
}

namespace TravBotSharp.Files.Tasks
{
    public enum ResTypeEnum
    {
        AllResources = 0,
        ExcludeCrop,
        OnlyCrop
    }
    public enum BuildingStrategyEnum
    {
        BasedOnRes = 0,
        BasedOnLevel,
        BasedOnProduction
    }
}
