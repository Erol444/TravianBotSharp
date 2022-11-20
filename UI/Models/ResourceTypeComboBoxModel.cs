using MainCore.Enums;
using MainCore.Helper;

namespace UI.Models
{
    public class ResourceTypeComboBoxModel
    {
        public ResTypeEnums Type { get; set; }
        public string Name => Type.ToString().EnumStrToString();
    }
}