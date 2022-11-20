using MainCore.Enums;
using System;

namespace UI.Models
{
    public class CurrentlyBuildingModel
    {
        public int Location { get; set; }
        public BuildingEnums Type { get; set; }
        public int Level { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}