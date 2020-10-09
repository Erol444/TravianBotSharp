using System.Collections.Generic;

namespace TravBotSharp.Files.Models.MapModels
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