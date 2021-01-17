using System;
using System.Collections.Generic;

namespace TbsCore.Models.MapModels
{
    public class MapPositionDataT4_4
    {
        public class Root
        {
            public Response response { get; set; }
        }

        public class Tile
        {
            public string x { get; set; }
            public string y { get; set; }
            public string c { get; set; }
            public string t { get; set; }
            public string d { get; set; }
            public string u { get; set; }
            public string a { get; set; }

            public MapTile GetMapTile()
            {
                return new MapTile()
                {
                    Coordinates = new Coordinates()
                    {
                        x = Int32.Parse(this.x),
                        y = Int32.Parse(this.y)
                    },
                    Title = this.c
                };
            }
        }

        public class Data
        {
            public List<Tile> tiles { get; set; }
        }

        public class Response
        {
            public bool error { get; set; }
            public object errorMsg { get; set; }
            public Data data { get; set; }
        }
    }
}