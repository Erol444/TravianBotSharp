using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Models.ResourceModels;
using TravBotSharp.Files.Tasks;

namespace TbsCore.Models.VillageModels
{
    /// <summary>
    /// BotTask that wasn't finished yet because there weren't enough resources
    /// </summary>
    public class VillUnfinishedTask
    {
        /// <summary>
        /// Resources needed for the task
        /// </summary>
        public Resources ResNeeded { get; set; }
        /// <summary>
        /// The task
        /// </summary>
        public BotTask Task { get; set; }
    }
}
