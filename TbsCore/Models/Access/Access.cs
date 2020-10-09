using System;

namespace TravBotSharp.Files.Models.AccModels
{
    public class Access : AccessRaw
    {
        /// <summary>
        /// Random selected user agent for this access
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// Is this account sitter. Used for functionality like changing village name
        /// </summary>
        public bool IsSittering { get; set; }
        /// <summary>
        /// When was this access last used
        /// </summary>
        public DateTime LastUsed { get; set; }
        /// <summary>
        /// If the access is ok
        /// </summary>
        public bool Ok { get; set; }
    }
}