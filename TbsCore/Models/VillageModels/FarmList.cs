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
            // 6 type for farming
            // no one (do they ?) use ram, cata, chief or settler
            // and hero ._.
            Troops = new int[6];
            Coord = new Coordinates();
        }

        public Farm(Farm f)
        {
            Troops = new int[6];
            for (int i = 0; i < 6; i++)
            {
                Troops[i] = f.Troops[i];
            }

            Coord = new Coordinates();
            Coord.x = f.Coord.x;
            Coord.y = f.Coord.y;
        }

        public Coordinates Coord { get; set; }
        public int[] Troops { get; set; }
    }
}