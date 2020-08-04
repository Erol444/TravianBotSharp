namespace TravBotSharp.Files.Models
{
    public class Adventure
    {
        public int TimeLeftSeconds { get; set; }
        public Coordinates Coordinates { get; set; }
        public int DurationSeconds { get; set; }
        public int Difficulty { get; set; }
        public string Ref { get; set; }
        public string AdventureId { get; set; }

    }
}
