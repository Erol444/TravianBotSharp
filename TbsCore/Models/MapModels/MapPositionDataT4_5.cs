using System;
using System.Collections.Generic;

namespace TbsCore.Models.MapModels
{
    public class MapPositionDataT4_5
    {
        public List<Tile> tiles { get; set; }
    }

    public class Position
    {
        public string x { get; set; }
        public string y { get; set; }
    }

    public class Tile
    {
        public Position position { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string uid { get; set; }
        public string aid { get; set; }
        public object did { get; set; }

        public MapTile GetMapTile()
        {
            return new MapTile()
            {
                Coordinates = new Coordinates()
                {
                    x = Int32.Parse(this.position.x),
                    y = Int32.Parse(this.position.y)
                },
                Title = this.title
            };
        }
    }
}
