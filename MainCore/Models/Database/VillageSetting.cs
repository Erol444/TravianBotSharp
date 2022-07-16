namespace MainCore.Models.Database
{
    public class VillageSetting
    {
        public int AccountId { get; set; }
        public int VillageId { get; set; }
        public int RefreshMin { get; set; }
        public int RefreshMax { get; set; }
    }
}