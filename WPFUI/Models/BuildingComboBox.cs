using MainCore.Enums;
using MainCore.Helper;

namespace WPFUI.Models
{
    public class BuildingComboBox
    {
        public BuildingEnums Building { get; set; }

        public string Name => Building.ToString().EnumStrToString();
    }
}