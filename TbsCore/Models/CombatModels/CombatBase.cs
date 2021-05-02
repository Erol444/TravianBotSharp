using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.CombatModels
{
    public class CombatBase
    {
        public Classificator.TribeEnum Tribe { get; set; }
        public int[] Troops { get; set; }
        public int[] Improvements { get; set; }
        public CombatHero Hero { get; set; }
    }
}
