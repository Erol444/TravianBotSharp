using MainCore.Enums;
using MainCore.Helper;

namespace MainCore.Models.Runtime
{
    public class PlanTask
    {
        public PlanTypeEnums Type { get; set; }
        public int Level { get; set; }

        // Build
        public int Location { get; set; }

        public BuildingEnums Building { get; set; }

        // Res
        public ResTypeEnums ResourceType { get; set; }

        public BuildingStrategyEnums BuildingStrategy { get; set; }

        public string Content => Type switch
        {
            PlanTypeEnums.General => $"Build {Building.ToString().EnumStrToString()} to level {Level} at location {Location}",
            PlanTypeEnums.ResFields => $"Auto build {ResourceType.ToString().EnumStrToString()} to level {Level} {BuildingStrategy.ToString().EnumStrToString().ToLower()}",
            _ => "",
        };
    }
}