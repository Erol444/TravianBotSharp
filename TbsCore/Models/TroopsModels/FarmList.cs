namespace TravBotSharp.Files.Models.TroopsModels
{
    public class FarmList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfFarms { get; set; }
        /// <summary>
        /// On which intervals should bot send this particular FL. If 1, every
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// Counter for the interval functionality
        /// </summary>
        public int IntervalCounter { get; set; }
        //settings
        public RaidStyle RaidStyle { get; set; }
        public bool Enabled { get; set; }
    }
    public enum RaidStyle
    {
        RaidSuccessful,
        RaidLosses,
        RaidLost
    }
}
