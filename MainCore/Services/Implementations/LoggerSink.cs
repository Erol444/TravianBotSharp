using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace MainCore.Services.Implementations
{
    public class LoggerSink : ILogEventSink
    {
        public Dictionary<string, LinkedList<LogEvent>> Logs { get; } = new();
        private readonly Dictionary<string, object> _objLocks = new();

        public event Action<LogEvent> LogEmitted;

        public void Emit(LogEvent logEvent)
        {
            var account = logEvent.Properties.GetValueOrDefault("Account")?.ToString();
            if (account is null) return;
            account = account.Replace('"', ' ').Trim();

            var listLog = Logs.GetValueOrDefault(account);
            if (listLog is null)
            {
                listLog = new LinkedList<LogEvent>();
                Logs.Add(account, listLog);
                _objLocks.Add(account, new object());
            }

            lock (_objLocks[account])
            {
                listLog.AddFirst(logEvent);
                // keeps 200 message
                while (listLog.Count > 200)
                {
                    listLog.RemoveLast();
                }

                LogEmitted?.Invoke(logEvent);
            }
        }
    }
}