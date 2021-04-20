using System.Collections.Generic;
using TbsCore.Models.MapModels;

namespace TbsCore.Models.VillageModels
{
    public class FarmList
    {
        public FarmList()
        {
            Targets = new List<Farm>();
        }

        public string Name { get; set; }
        public List<Farm> Targets { get; set; }
    }

    public class Farm
    {
        public Farm()
        {
        }

        public Farm(int[] troops, Coordinates coords)
        {
            Troops = troops;
            Coords = coords;
        }

        public Coordinates Coords { get; set; }
        public int[] Troops { get; set; }
    }
}