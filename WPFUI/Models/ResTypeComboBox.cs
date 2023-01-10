using MainCore;
using MainCore.Enums;

namespace WPFUI.Models
{
    public class ResTypeComboBox
    {
        public ResTypeEnums Type { get; set; }
        public string Name => Type.ToString().EnumStrToString();
    }
}