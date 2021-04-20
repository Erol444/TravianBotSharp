using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.BuildingModels
{
    public class DemolishTask
    {
        public Classificator.BuildingEnum Building { get; set; }
        public int BuildingId { get; set; }
        public int Level { get; set; }
    }
}