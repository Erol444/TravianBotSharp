using MainCore;
using MainCore.Enums;

namespace WPFUI.Models
{
    public class ResTypeComboBox
    {
        public ResTypeComboBox(ResTypeEnums type)
        {
            Type = type;
        }

        public ResTypeEnums Type { get; set; }
        public string Name => Type.ToString().EnumStrToString();
    }
}