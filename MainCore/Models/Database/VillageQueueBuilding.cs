namespace MainCore.Models.Database
{
    public class VillageQueueBuilding
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public int Location { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
    }
}