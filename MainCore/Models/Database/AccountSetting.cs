namespace MainCore.Models.Database
{
    public class AccountSetting
    {
        public int AccountId { get; set; }
        public int ClickDelayMin { get; set; }
        public int ClickDelayMax { get; set; }
        public int TaskDelayMin { get; set; }
        public int TaskDelayMax { get; set; }
        public int WorkTimeMin { get; set; }
        public int WorkTimeMax { get; set; }
        public int SleepTimeMin { get; set; }
        public int SleepTimeMax { get; set; }
        public bool IsDontLoadImage { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsClosedIfNoTask { get; set; }
        public bool IsAutoAdventure { get; set; }
    }
}