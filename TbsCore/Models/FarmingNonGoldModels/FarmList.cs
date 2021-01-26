using System.Collections.Generic;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;

namespace TbsCore.Models.FarmingNonGoldModels
{
    public class FarmList
    {
        public FarmList()
        {
            targets = new List<Coordinates>();
        }

        public List<Coordinates> targets { get; set; }
        public List<TroopsRaw> troops { get; set; }
    }
}