using TravBotSharp.Files.Models.SideBarModels;

namespace TbsCore.Models.SideBarModels
{
    public class Quest
    {
        /// <summary>
        /// Category of the quest
        /// </summary>
        public Category category { get; set; }
        /// <summary>
        /// Is the quest already finished
        /// </summary>
        public bool finished { get; set; }
        /// <summary>
        /// Id of the quest
        /// </summary>
        public string Id { get; set; }
    }
}

namespace TravBotSharp.Files.Models.SideBarModels
{
    public enum Category
    {
        Battle,
        Economy,
        World
    }
}
