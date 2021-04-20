using TbsCore.Models.MapModels;

namespace TbsCore.Models.VillageModels
{
    public class NewVillage
    {
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public bool SettlersSent { get; set; }
    }
}