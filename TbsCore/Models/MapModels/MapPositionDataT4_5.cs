using System;
using System.Collections.Generic;
using System.Text;

namespace TbsCore.Models.MapModels
{
    class MapPositionDataT4_5
    {
        public class Position
        {
            public string x { get; set; }
            public string y { get; set; }
        }

        public class Tile
        {
            public Position position { get; set; }
            public string text { get; set; }
            public string title { get; set; }
        }

        public class Root
        {
            public List<Tile> tiles { get; set; }
        }
    }
}
