using TbsCrossPlatform.Models.Enums;

namespace TbsCrossPlatform.Models
{
    public class Adventure
    {
        public int TimeLeftSeconds { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int DurationSeconds { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public string Ref { get; set; }
        public string AdventureId { get; set; }
    }
}