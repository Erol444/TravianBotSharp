using MainCore.Enums;
using MainCore.Helper.Implementations;

namespace WPFUI.Models
{
    public class BuildingStrategyComboBox
    {
        public BuildingStrategyEnums Strategy { get; set; }
        public string Name => Strategy.ToString().EnumStrToString();
    }
}