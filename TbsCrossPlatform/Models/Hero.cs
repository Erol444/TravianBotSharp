using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbsCrossPlatform.Models
{
    public class Hero
    {
        public int AccountId { get; set; }
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