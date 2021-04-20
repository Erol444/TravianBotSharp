using System.Collections.Generic;
using TbsCore.Models.SideBarModels;

namespace TbsCore.Models.Settings
{
    public class QuestsSettings
    {
        /// <summary>
        ///     Whether bot should claim daily quests
        /// </summary>
        public bool ClaimDailyQuests { get; set; }

        /// <summary>
        ///     Whether bot should claim beginner quests
        /// </summary>
        public bool ClaimBeginnerQuests { get; set; }

        /// <summary>
        ///     In which village should it claim
        /// </summary>
        public int VillToClaim { get; set; }

        /// <summary>
        ///     List of beginner quests
        /// </summary>
        public List<Quest> Quests { get; set; }

        public void Init()
        {
            Quests = new List<Quest>();

            ClaimDailyQuests = true;
            ClaimBeginnerQuests = true;
        }
    }
}