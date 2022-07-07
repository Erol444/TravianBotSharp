namespace MainCore.Models.Database
{
    public class VillageBuilding
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public bool IsUnderConstruction { get; set; }
    }
}