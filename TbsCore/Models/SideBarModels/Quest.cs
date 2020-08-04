namespace TravBotSharp.Files.Models.SideBarModels
{
    public class Quest
    {
        public Category category { get; set; }
        public byte level { get; set; }
        public bool finished { get; set; }
    }
    public enum Category
    {
        Battle,
        Economy,
        World
    }
}
