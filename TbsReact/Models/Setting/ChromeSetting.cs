namespace TbsReact.Models
{
    public class ChromeSetting
    {
        public Delay Click { get; set; }
        public Delay WorkTime { get; set; }
        public Delay SleepTime { get; set; }
        public bool DisableImages { get; set; }
        public bool AutoClose { get; set; }
    }

    public class Delay
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }
}