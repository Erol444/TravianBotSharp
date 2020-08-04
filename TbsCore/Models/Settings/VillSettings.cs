using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Models.Settings
{
    public class VillSettings
    {
        public void Init()
        {
            GetRes = true;
            SendRes = false;
        }
        public VillType Type { get; set; }
        public Classificator.TroopsEnum BarracksTrain { get; set; }
        public bool GreatBarracksTrain { get; set; }
        public Classificator.TroopsEnum StableTrain { get; set; }
        public bool GreatStableTrain { get; set; }
        public Classificator.TroopsEnum WorkshopTrain { get; set; }
        public bool GetRes { get; set; }
        public bool SendRes { get; set; }
    }
    public enum VillType
    {
        Farm,
        Support,
        Deff,
        Off
    }
}
