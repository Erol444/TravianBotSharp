using MainCore.Enums;
using MainCore.Helper;

namespace WPFUI.Models
{
    public class ResTypeComboBox
    {
        public ResTypeEnums Type { get; set; }
        public string Name => Type.ToString().EnumStrToString();
    }
}