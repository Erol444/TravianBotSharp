namespace MainCore.Models.Database
{
    public class VillageProduction
    {
        public int VillageId { get; set; }
        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }
    }
}