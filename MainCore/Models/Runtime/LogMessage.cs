using MainCore.Enums;
using System;

namespace MainCore.Models.Runtime
{
    public class LogMessage
    {
        public DateTime DateTime { get; set; }
        public LevelEnum Level { get; set; }
        public string Message { get; set; }
    }
}