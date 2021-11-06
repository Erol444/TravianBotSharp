using System;
using System.Collections.Generic;

using TbsCore.Models.MapModels;
using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.Settings
{
    public class TradeRouteSettings
    {
        public void Init()
        {
            TradeRoutes = new List<TradeRoute>();
            Next = 0;
        }

        public List<TradeRoute> TradeRoutes { get; set; }
        public int Next { get; set; }
    }

    public class TradeRoute
    {
        public Coordinates Location { get; set; }
        public Resources Resource { get; set; }
        public int Time { get; set; }
        public int TimeDelay { get; set; }
        public DateTime Last { get; set; }
        public bool Active { get; set; }
    }
}