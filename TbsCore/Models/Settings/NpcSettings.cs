using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.Models.Settings
{
    public class NpcSettings
    {
        public void Init()
        {
            ResourcesRatio = new Resources();
        }
        public bool Enabled { get; set; }
        public bool NpcIfOverflow { get; set; }
        public Resources ResourcesRatio { get; set; }
    }
}
