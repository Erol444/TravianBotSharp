using Serilog.Events;
using System;

namespace MainCore.Models.Runtime
{
    public class LogMessage
    {
        public LogMessage(LogEvent logEvent)
        {
            DateTime = logEvent.Timestamp.DateTime;
            Level = logEvent.Level;
            var message = logEvent.RenderMessage(null);
            Message = message.Replace('"', ' ').Trim();
        }

        public DateTime DateTime { get; set; }
        public LogEventLevel Level { get; set; }
        public string Message { get; set; }
    }
}