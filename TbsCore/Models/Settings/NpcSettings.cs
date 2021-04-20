using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.Settings
{
    public class NpcSettings
    {
        public bool Enabled { get; set; }
        public bool NpcIfOverflow { get; set; }
        public Resources ResourcesRatio { get; set; }

        public void Init()
        {
            ResourcesRatio = new Resources();
        }
    }
}