using TbsCore.Models.MapModels;

namespace TbsCore.Models
{
    public class InactiveFarm
    {
        public Coordinates coord { get; set; }
        public int distance { get; set; }
        public string namePlayer { get; set; }
        public string nameAlly { get; set; }
        public string nameVill { get; set; }
        public int population { get; set; }
    }
}