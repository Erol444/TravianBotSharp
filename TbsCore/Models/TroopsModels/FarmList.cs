using System.Collections.Generic;
using TbsCore.Models.MapModels;

namespace TbsCore.Models.TroopsModels
{
    public class FarmList
    {
        public void Init()
        {
            this.Farms = new List<GoldClubFarm>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfFarms { get; set; }

        /// <summary>
        /// On which intervals should bot send this particular FL. If 1, every
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Counter for the interval functionality
        /// </summary>
        public int IntervalCounter { get; set; }

        //settings
        public RaidStyle RaidStyle { get; set; }

        public bool Enabled { get; set; }

        public List<GoldClubFarm> Farms { get; set; }
    }

    public class GoldClubFarm
    {
        public GoldClubFarm(Coordinates coords)
        {
            this.Coordinates = coords;
        }
        public Coordinates Coordinates { get; set; }
        //public TroopsBase Troops { get; set; }
    }
}

namespace TbsCore.Models.TroopsModels
{
    public enum RaidStyle
    {
        RaidSuccessful,
        RaidLosses,
        RaidLost
    }
}