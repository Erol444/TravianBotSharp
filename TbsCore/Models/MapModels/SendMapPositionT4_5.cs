using System.Collections.Generic;

namespace TbsCore.Models.MapModels
{
    public class SendMapPositionT4_5
    {
        public class Root
        {
            public Data data { get; set; }
        }

        public class Data
        {
            public int x { get; set; }
            public int y { get; set; }
            public int zoomLevel { get; set; }
            public List<object> ignorePositions { get; set; }
        }
    }
}