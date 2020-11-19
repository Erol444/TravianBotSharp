using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TbsCore.Helpers
{
    public class TbsLoggerSink : ILogEventSink
    {
        public event EventHandler NewLogHandler;

        public TbsLoggerSink() { }

        public void Emit(LogEvent logEvent)
        {
#if DEBUG
            Console.WriteLine($"{logEvent.Timestamp}] {logEvent.MessageTemplate}");
#endif
            NewLogHandler?.Invoke(typeof(TbsCore.Helpers.TbsLoggerSink), new LogEventArgs() { Log = logEvent });
        }
    }

    public class LogEventArgs : EventArgs
    {
        public LogEvent Log { get; set; }
    }
}
