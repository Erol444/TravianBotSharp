namespace WPFUI.Models
{
    public class Building
    {
        public int Location { get; set; }
        public int Type { get; set; }
        public string Name => Type.ToString();
    }
}