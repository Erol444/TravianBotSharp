using System;
using System.IO;
using System.Collections.Generic;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Configuration;
using Serilog.Formatting.Display;

namespace TbsCore.Models.Logging
{
    public class TbsSink : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter;
        private readonly LogOutput _logs;
        private readonly object _syncRoot = new object();

        public TbsSink(LogOutput logs, ITextFormatter textFormatter)
        {
            if (textFormatter == null) throw new ArgumentNullException(nameof(textFormatter));

            _logs = logs;
            _textFormatter = textFormatter;
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_syncRoot)
            {
                var stringWriter = new StringWriter();

                _textFormatter.Format(logEvent, stringWriter);
                LogEventPropertyValue username;
                logEvent.Properties.TryGetValue("Username", out username);
                _logs.Add(username.ToString(), stringWriter.ToString());
            }
        }
    }
}

namespace Serilog
{
    public static class TBSLogExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        public static LoggerConfiguration TbsSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  TbsCore.Models.Logging.LogOutput logs,
                  LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
                  string outputTemplate = DefaultOutputTemplate,
                  IFormatProvider formatProvider = null,
                  LoggingLevelSwitch levelSwitch = null)
        {
            if (logs == null) throw new ArgumentNullException(nameof(logs));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var sink = new TbsCore.Models.Logging.TbsSink(logs, formatter);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }

        public static LoggerConfiguration TbsSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  ITextFormatter formatter,
                  TbsCore.Models.Logging.LogOutput logs,
                  LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
                  LoggingLevelSwitch levelSwitch = null)
        {
            if (logs == null) throw new ArgumentNullException(nameof(logs));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var sink = new TbsCore.Models.Logging.TbsSink(logs, formatter);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}