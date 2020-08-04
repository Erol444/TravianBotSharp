using System;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Models.VillageModels
{
    public class BuildingCurrently
    {
        public DateTime Duration { get; set; }
        public byte Level { get; set; }
        public BuildingEnum Building { get; set; }
    }
}
