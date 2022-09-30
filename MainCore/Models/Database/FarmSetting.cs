namespace MainCore.Models.Database
{
    public class FarmSetting
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int IntervalMin { get; set; }
        public int IntervalMax { get; set; }
    }
}