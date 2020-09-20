using System;
using System.Collections.Generic;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.Settings;

namespace TravBotSharp.Files.Models.AccModels
{
    public class Hero
    {
        public void init()
        {
            HeroInfo = new HeroInfo();
            Settings = new HeroSettings();
            Settings.Init();
            Adventures = new List<Adventure>();
            Items = new List<HeroItem>();
            Equipt = new Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum>();
        }
        public List<Adventure> Adventures { get; set; }
        /// <summary>
        /// Here only because TTWars doesn't register hero is on away immediately.
        /// </summary>
        public DateTime NextHeroSend { get; set; }
        /// <summary>
        /// Parsed from /hero.php, when the arrival will be
        /// </summary>
        public DateTime HeroArrival { get; set; }
        /// <summary>
        /// In which village should bot revive the hero
        /// </summary>
        public int ReviveInVillage { get; set; }
        public int AdventureNum { get; set; }
        public StatusEnum Status { get; set; }
        public int HomeVillageId { get; set; }
        public HeroInfo HeroInfo { get; set; }
        public HeroSettings Settings { get; set; }
        /// <summary>
        /// Hero items in the inventory
        /// </summary>
        public List<HeroItem> Items { get; set; }
        /// <summary>
        /// Items currently equipt
        /// </summary>
        public Dictionary<Classificator.HeroItemCategory, Classificator.HeroItemEnum> Equipt { get; set; }


        public enum StatusEnum
        {
            Unknown,
            Home,
            Away,
            Dead,
            Regenerating
        }
        //TODO: SELL HERO STUFF, EQUIP HERO!!
    }
}
