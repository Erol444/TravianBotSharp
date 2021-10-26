using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Helpers;
using TbsCore.Models.ResourceModels;

namespace TbsCore.Models.AttackModels
{
    /// <summary>
    /// Report of an attack
    /// </summary>
    public class AttackReport
    {
        // TODO: Attacker (nickname, id), Attacker Village (id, coords, name), Deffender (nickname, id), Deffending village (id, coords, name)
        // Attacker troops, killed, tribe, additional information (bounty / scouting report / ram&cata report)
        // Deffender List<troops, tribe, killed>

        /// <summary>
        /// Resources raided / scouted
        /// </summary>
        public Resources Resources { get; set; }
        /// <summary>
        /// When scouting for resources, you see cranny size as well
        /// </summary>
        public int CrannySize { get; set; }
        /// <summary>
        /// When scouting for deffense, you see residence/palace and wall level as well
        /// </summary>
        /// 

        public int ResidenceLevel {get;set;}
        public int WallLevel { get; set; }

        /// <summary>
        /// Calculates resources that can be raided
        /// </summary>
        public Resources GetRaidableRes()
        {
            long[] res = new long[4];
            var arr = this.Resources.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                res[i] = arr[i] - this.CrannySize;
                if (res[i] < 0) res[i] = 0;
            }
            return ResourcesHelper.ArrayToResources(res);
        }
    }
}
