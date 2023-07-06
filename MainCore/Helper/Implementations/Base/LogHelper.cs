using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class LogHelper : ILogHelper
    {
        private readonly Dictionary<int, ILogger> _loggers = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventManager _eventManager;
        private readonly LoggerSink _loggerSink;

        public LogHelper(IDbContextFactory<AppDbContext> contextFactory, IServiceProvider serviceProvider, ILogEventSink loggerSink, IEventManager eventManager)
        {
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
            _loggerSink = loggerSink as LoggerSink;
            _eventManager = eventManager;

            _loggerSink.LogEmitted += OnLogEmitted;
        }

        public void Init()
        {
            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Services(_serviceProvider)
              .WriteTo.Map("Account", "Other", (acc, wt) =>
                    wt.File($"./logs/log-{acc}-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
              .CreateLogger();
        }

        public void Shutdown()
        {
            Log.CloseAndFlush();
        }

        public LinkedList<LogMessage> GetLog(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(accountId);
            var accountUsername = account.Username;

            var logs = _loggerSink.Logs.GetValueOrDefault(accountUsername);
            if (logs is null) return new LinkedList<LogMessage>();
            var copiedLogs = logs.ToList();
            return new LinkedList<LogMessage>(logs.Select(x => new LogMessage(x)));
        }

        private void OnLogEmitted(LogEvent logEvent)
        {
            var accountUsername = logEvent.Properties.GetValueOrDefault("Account")?.ToString();
            accountUsername = accountUsername.Replace('"', ' ').Trim();
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.FirstOrDefault(x => x.Username.Equals(accountUsername));
            _eventManager.OnLogUpdate(account?.Id ?? 0, new LogMessage(logEvent));
        }

        public void Information(int accountId, string message)
        {
            var logger = _loggers.GetValueOrDefault(accountId);
            if (logger is null)
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);
                logger = Log.ForContext("Account", account.Username);
                _loggers.Add(accountId, logger);
            }

            logger.Information("{message}", message);
        }

        public void Information(int accountId, string message, BotTask task) => Information(accountId, $"[{task.GetName()}] {message}");

        public void Warning(int accountId, string message)
        {
            var logger = _loggers.GetValueOrDefault(accountId);
            if (logger is null)
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);
                logger = Log.ForContext("Account", account.Username);
                _loggers.Add(accountId, logger);
            }

            logger.Warning("{message}", message);
        }

        public void Warning(int accountId, string message, BotTask task) => Warning(accountId, $"[{task.GetName()}] {message}");

        public void Error(int accountId, string message)
        {
            var logger = _loggers.GetValueOrDefault(accountId);
            if (logger is null)
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);
                logger = Log.ForContext("Account", account.Username);
                _loggers.Add(accountId, logger);
            }

            logger.Error("{message}", message);
        }

        public void Error(int accountId, string message, BotTask task) => Error(accountId, $"[{task.GetName()}] {message}");

        public void Error(int accountId, string message, Exception error)
        {
            var logger = _loggers.GetValueOrDefault(accountId);
            if (logger is null)
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);
                logger = Log.ForContext("Account", account.Username);
                _loggers.Add(accountId, logger);
            }

            logger.Error(error, "{message}", message);
        }
    }
}