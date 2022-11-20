using MainCore.Enums;
using MainCore.Helper;

namespace UI.Models
{
    public class StrategyComboBoxModel
    {
        public BuildingStrategyEnums Strategy { get; set; }
        public string Name => Strategy.ToString().EnumStrToString();
    }
}