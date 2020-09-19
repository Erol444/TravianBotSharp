namespace TravBotSharp.Files.Models.Settings
{
    public class HeroSettings
    {
        public void Init()
        {
            AutoReviveHero = true;
            AutoSendToAdventure = true;
            MinHealth = 15;
            MaxDistance = 30;
            Upgrades = new byte[4] { 2, 0, 0, 2 } ;
        }
        public bool AutoReviveHero { get; set; }
        public bool AutoSendToAdventure { get; set; }
        public int MinHealth { get; set; }
        public int MaxDistance { get; set; }
        public int MaxTime { get; set; } //in seconds!
        /// <summary>
        /// Auto refresh hero information every 1 hour
        /// </summary>
        public bool AutoRefreshInfo { get; set; } = true;
        /// <summary>
        /// Auto use resources from hero inventory
        /// </summary>
        public bool AutoUseRes { get; set; } = true;
        /// <summary>
        /// Auto equip hero with better items
        /// </summary>
        public bool AutoEquip { get; set; } = true;
        public bool BuyAdventures { get; set; }

        /// <summary>
        /// To automatically set hero points on new level
        /// </summary>
        public bool AutoSetPoints { get; set; }
        /// <summary>
        /// What attributes (in what amount) should be improved on auto set hero points
        /// </summary>
        public byte[] Upgrades { get; set; }
    }
}
