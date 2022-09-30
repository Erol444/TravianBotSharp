using MainCore.Enums;
using MainCore.Helper;

namespace WPFUI.Models
{
    public class BuildingStrategyComboBox
    {
        public BuildingStrategyEnums Strategy { get; set; }
        public string Name => Strategy.ToString().EnumStrToString();
    }
}