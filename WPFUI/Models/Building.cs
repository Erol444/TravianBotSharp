using MainCore.Enums;

namespace WPFUI.Models
{
    public class Building
    {
        public int Location { get; set; }
        public BuildingEnums Type { get; set; }
        public string Name => Type.ToString();
    }
}