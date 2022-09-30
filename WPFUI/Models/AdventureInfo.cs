namespace WPFUI.Models
{
    public class AdventureInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Coordinates => $"{X}|{Y}";
        public string Difficulty { get; set; }
    }
}