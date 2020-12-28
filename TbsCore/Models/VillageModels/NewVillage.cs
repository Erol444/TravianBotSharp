using TbsCore.Models.MapModels;

namespace TbsCore.Models.VillageModels
{
    public class NewVillage
    {
        public string Name { get; set; }
        public Coordinates coordinates { get; set; }
        public bool SettlersSent { get; set; }
    }
}
