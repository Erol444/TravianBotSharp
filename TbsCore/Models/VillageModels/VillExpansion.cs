using System;

namespace TravBotSharp.Files.Models.VillageModels
{

    public class VillExpansion
    {
        /// <summary>
        /// Whether there are 3 settlers available for expansion
        /// </summary>
        public bool ExpansionAvailable { get; set; }

        // Expansions available/ Used for this village. Can be viewed in residence/palace or with plus account
        // in village statistics
        public int ExpansionsAvailable { get; set; }
        public int ExpansionsUsed { get; set; }

        /// <summary>
        /// Whether bot automatically starts celebrations
        /// </summary>
        public bool AutoCelebrations { get; set; }
        /// <summary>
        /// Auto-Celebrate big celebrations. Otherwise small celebrations
        /// </summary>
        public bool BigCelebrations { get; set; }
        /// <summary>
        /// When will the current celebration finish
        /// </summary>
        public DateTime CelebrationEnd { get; set; }
    }
}