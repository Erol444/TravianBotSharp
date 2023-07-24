using MainCore.Enums;

namespace WPFUI.Models
{
    public class TribeComboBox
    {
        public TribeEnums Tribe { get; set; }
        public string Name => Tribe.ToString();
    }
}