using System;

namespace TbsCore.Models.Settings
{
    /// <summary>
    /// Data about when things last happened
    /// </summary>
    public class TimingData
    {
        public void Init() { }
        public DateTime NextHeroRefresh { get; set; }
    }
}
