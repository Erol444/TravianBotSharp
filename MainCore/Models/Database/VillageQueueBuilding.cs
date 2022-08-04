using MainCore.Enums;

namespace MainCore.Models.Database
{
    public class VillageQueueBuilding
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public int Item { get; set; }
        public int Location { get; set; }
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
    }
}