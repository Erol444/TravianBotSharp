namespace WPFUI.Models
{
    public class VillageSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coords { get; set; }
        public int TimeUpdate { get; set; }
        public int TimeUpdateRange { get; set; }
        public bool IsInstantUpgrade { get; set; }
        public int InstantTime { get; set; }
    }
}