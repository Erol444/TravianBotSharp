using MainCore.Enums;
using MainCore.Helper.Implementations;

namespace WPFUI.Models
{
    public class ResTypeComboBox
    {
        public ResTypeEnums Type { get; set; }
        public string Name => Type.ToString().EnumStrToString();
    }
}