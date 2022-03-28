using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.IO;

namespace TbsCore.Models.Logging
{
    public class TbsSink : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter;

        public TbsSink(ITextFormatter textFormatter)
        {
            _textFormatter = textFormatter;
        }

        public void Emit(LogEvent logEvent)
        {
            var stringWriter = new StringWriter();

            _textFormatter.Format(logEvent, stringWriter);
            logEvent.Properties.TryGetValue("Username", out LogEventPropertyValue username);
            // toString problem, it turn {"username"} to \"username\"
            // replace here to solve that
            LogOutput.Instance.Add(username.ToString().Replace("\"", ""), stringWriter.ToString());
        }
    }

    public static class TbsSinkExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

        public static LoggerConfiguration TbsSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
                  string outputTemplate = DefaultOutputTemplate,
                  IFormatProvider formatProvider = null,
                  LoggingLevelSwitch levelSwitch = null)
        {
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var sink = new TbsSink(formatter);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}