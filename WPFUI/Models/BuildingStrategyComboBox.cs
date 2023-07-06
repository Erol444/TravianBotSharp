using MainCore;
using MainCore.Enums;

namespace WPFUI.Models
{
    public class BuildingStrategyComboBox
    {
        public BuildingStrategyComboBox(BuildingStrategyEnums strategy)
        {
            Strategy = strategy;
        }

        public BuildingStrategyEnums Strategy { get; set; }
        public string Name => Strategy.ToString().EnumStrToString();
    }
}