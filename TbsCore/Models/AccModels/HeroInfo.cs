using System;

namespace TravBotSharp.Files.Models.AccModels
{
    public class HeroInfo
    {
        public int Health { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int AvaliblePoints { get; set; }
        public bool NewLevel { get; set; }
        public int FightingStrengthPoints { get; set; }
        public int OffBonusPoints { get; set; }
        public int DeffBonusPoints { get; set; }
        public int ResourcesPoints { get; set; }
        public int HeroProduction { get; set; }
        public byte SelectedResource { get; set; }
    }
}
