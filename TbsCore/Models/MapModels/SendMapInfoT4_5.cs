using System.Collections.Generic;

namespace TbsCore.Models.MapModels
{
    public class SendMapInfoT4_5
    {
        public class Root
        {
            public List<Datum> data { get; set; }
            public int zoomLevel { get; set; }
        }
        public class Position
        {
            public int x0 { get; set; }
            public int y0 { get; set; }
            public int x1 { get; set; }
            public int y1 { get; set; }
        }

        public class Datum
        {
            public Position position { get; set; }
        }
    }
}
