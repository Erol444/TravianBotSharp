namespace WPFUI.Models
{
    public class VillageSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coords { get; set; }
        public bool IsUseHeroRes { get; set; }
        public bool IsInstantComplete { get; set; }
        public int InstantCompleteTime { get; set; }
    }
}