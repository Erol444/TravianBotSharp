using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.CombatModels
{
    public class CombatHero
    {
        /// <summary>
        /// Hero power, offensive / deffensive bonus
        /// </summary>
        public HeroInfo Info { get; set; }
        
        /// <summary>
        /// Hero items (natar horn, shield, right hand items)
        /// </summary>
        public Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum> Items { get; set; }
    }
}