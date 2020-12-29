using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.Settings;

namespace TbsCore.Models.Settings
{
    public class VillSettings
    {
        public void Init()
        {
            GetRes = true;
            SendRes = false;
            UseHeroRes = true;
        }
        public VillType Type { get; set; }
        public Classificator.TroopsEnum BarracksTrain { get; set; }
        public bool GreatBarracksTrain { get; set; }
        public Classificator.TroopsEnum StableTrain { get; set; }
        public bool GreatStableTrain { get; set; }
        public Classificator.TroopsEnum WorkshopTrain { get; set; }
        public bool GetRes { get; set; }
        public bool SendRes { get; set; }
        public bool AutoExpandStorage { get; set; }
        public bool UseHeroRes { get; set; }
    }
}

namespace TravBotSharp.Files.Models.Settings
{
    public enum VillType
    {
        Farm,
        Support,
        Deff,
        Off
    }
}
