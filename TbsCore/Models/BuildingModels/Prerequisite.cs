using TbsCore.Helpers;

namespace TbsCore.Models.BuildingModels
{
    public class Prerequisite
    {
        public Classificator.BuildingEnum Building { get; set; }
        public int Level { get; set; }
    }
}