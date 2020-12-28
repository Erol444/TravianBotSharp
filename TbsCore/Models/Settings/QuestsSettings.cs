using System;
using System.Collections.Generic;
using TravBotSharp.Files.Models.SideBarModels;

namespace TravBotSharp.Files.Models.AccModels
{
    public class QuestsSettings
    {
        public  void Init()
        {
            Quests = new List<Quest>();
        }
        /// <summary>
        /// Whether bot should claim daily quests
        /// </summary>
        public bool ClaimDailyQuests { get; set; }
        /// <summary>
        /// Whether bot should claim beginner quests
        /// </summary>
        public bool ClaimBeginnerQuests { get; set; }
        /// <summary>
        /// In which village should it claim
        /// </summary>
        public int VillToClaim { get; set; }
        /// <summary>
        /// List of beginner quests
        /// </summary>
        public List<Quest> Quests { get; set; }
    }
}