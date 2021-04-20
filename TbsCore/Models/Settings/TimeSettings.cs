namespace TbsCore.Models.Settings
{
    public class TimeSettings
    {
        /// <summary>
        ///     Minimal work time for bot. In minutes.
        /// </summary>
        public int MinWork { get; set; }

        /// <summary>
        ///     Maximal work time for bot. In minutes.
        /// </summary>
        public int MaxWork { get; set; }

        /// <summary>
        ///     Minimal sleep time for bot. In minutes.
        /// </summary>
        public int MinSleep { get; set; }

        /// <summary>
        ///     Maximal sleep time for bot. In minutes.
        /// </summary>
        public int MaxSleep { get; set; }

        public void Init()
        {
            MinWork = 200;
            MaxWork = 300;
            MinSleep = 30;
            MaxSleep = 90;
        }
    }
}