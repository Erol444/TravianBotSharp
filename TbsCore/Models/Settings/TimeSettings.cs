using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Models.AccModels;

namespace TbsCore.Models.Settings
{
    public class TimeSettings
    {
        public void Init()
        {
            this.MinWork = 200;
            this.MaxWork = 300;
            this.MinSleep = 30;
            this.MaxSleep = 90;
        }
        /// <summary>
        /// Minimal work time for bot. In minutes.
        /// </summary>
        public int MinWork { get; set; }
        /// <summary>
        /// Maximal work time for bot. In minutes.
        /// </summary>
        public int MaxWork { get; set; }
        /// <summary>
        /// Minimal sleep time for bot. In minutes.
        /// </summary>
        public int MinSleep { get; set; }
        /// <summary>
        /// Maximal sleep time for bot. In minutes.
        /// </summary>
        public int MaxSleep { get; set; }
    }
}
