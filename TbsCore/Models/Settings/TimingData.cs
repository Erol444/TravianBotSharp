using System;

namespace TbsCore.Models.Settings
{
    /// <summary>
    ///     Data about when things last happened
    /// </summary>
    public class TimingData
    {
        public DateTime NextHeroRefresh { get; set; }

        public void Init()
        {
        }
    }
}