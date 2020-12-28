using System;
using System.Collections.Generic;
using System.Text;

namespace TbsCore.Models
{
    class TB
    {
        public class BuildingDemolish
        {
            public int stufe { get; set; }
            public int gid { get; set; }
            public int aid { get; set; }
            public string remain { get; set; }
        }

        public class Command
        {
            /// <summary>
            /// 1 = normal build
            /// 4 = Auto build res fields, based on lvl
            /// 5 = Auto build res fields, based on production
            /// 6 = Auto build res fields, based on storage
            /// </summary>
            public int cmdType { get; set; }
            /// <summary>
            /// Level of the building
            /// </summary>
            public int level { get; set; }
            /// <summary>
            /// Location id
            /// </summary>
            public int gid { get; set; }
            /// <summary>
            /// Building id
            /// If cmdType = 4/5/6:
            /// 60 = AllRes
            /// 61 = Exclude crop
            /// 62 = only crop
            /// </summary>
            public int bid { get; set; }
            public bool canMoveUp { get; set; }
            public bool canMoveDown { get; set; }
            public bool canDelete { get; set; }
            public bool canBuild { get; set; }
            public bool bReward { get; set; }
            public string mustBuildTime { get; set; }
        }

        public class TbRoot
        {
            public List<object> building_contract { get; set; }
            public string dorf1RefreshDate { get; set; }
            public string dorf2RefreshDate { get; set; }
            public string dorf1StartBuildDate { get; set; }
            public string dorf2StartBuildDate { get; set; }
            public string demolishEndDate { get; set; }
            public BuildingDemolish building_demolish { get; set; }
            public List<Command> commands { get; set; }
            public List<bool> resBonusBuilding { get; set; }
        }
    }
}
