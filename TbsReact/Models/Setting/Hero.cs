namespace TbsReact.Models.Setting
{
    public class Hero
    {
        public AutoAdventure AutoAdventure { get; set; }
        public AutoRefresh AutoRefresh { get; set; }
        public AutoPoint AutoPoint { get; set; }
        public AutoRevive AutoRevive { get; set; }
    }

    public class AutoAdventure
    {
        public bool IsActive { get; set; }

        public int MinHealth { get; set; }
        public int MaxDistance { get; set; }
    }

    public class AutoRefresh
    {
        public bool IsActive { get; set; }
        public Range Frequency { get; set; }
    }

    public class AutoRevive
    {
        public bool IsActive { get; set; }
        public int VillageId { get; set; }
    }

    public class AutoPoint
    {
        public bool IsActive { get; set; }
        public byte[] Points { get; set; }
    }
}