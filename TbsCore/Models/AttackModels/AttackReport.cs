using System.Collections.Generic;
using TbsCore.Helpers;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.TroopsModels;

namespace TbsCore.Models.AttackModels
{
    /// <summary>
    /// Report of an attack TODO: combine with CombatModels, DRY
    /// </summary>
    public class AttackReport
    {
        public AttackReport(bool init = false)
        {
            if (init)
            {
                Deffender = new CombatParticipant();
                Attacker = new CombatParticipant();
                AttackerArmy = new CombatTribeParticipant();
                DeffenderArmy = new List<CombatTribeParticipant>();
            }
        }
        public CombatParticipant Deffender { get; set; }
        public CombatParticipant Attacker { get; set; }

        public CombatTribeParticipant AttackerArmy { get; set; }
        public List<CombatTribeParticipant> DeffenderArmy { get; set; }

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

        public int ResidenceLevel { get; set; }
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

    public class CombatParticipant
    {
        public string Username { get; set; }
        public int UserId { get; set; }
        public string VillageName { get; set; }
        public int VillageId { get; set; }
    }
    public class CombatTribeParticipant : TroopsBase
    {
        public int[] Casualties { get; set; }
    }
}
