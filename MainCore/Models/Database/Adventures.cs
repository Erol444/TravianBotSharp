using MainCore.Enums;

namespace MainCore.Models.Database
{
    public class Adventure
    {
        public int AccountId { get; set; }
        public string AdventureId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DifficultyEnums Difficulty { get; set; }
    }
}