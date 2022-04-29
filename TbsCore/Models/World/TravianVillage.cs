using TbsCore.Models.MapModels;

namespace TbsCore.Models.AttackModels
{
    public class TravianVillage
    {
        // TODO Artifacts, Oasis
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Capital { get; set; }
        public int Population { get; set; }
        public Coordinates Coordinates { get; set; }
    }
}
