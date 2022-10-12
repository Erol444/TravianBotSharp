using MainCore.Enums;
using MainCore.Models.Runtime;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;

namespace MainCore.Services
{
    public sealed class LogManager : ILogManager
    {
        public LogManager(EventManager EventManager, IDbContextFactory<AppDbContext> contextFactory)
        {
            _eventManager = EventManager;
            _contextFactory = contextFactory;
        }

        public void Init()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Map("Account", "Other", (acc, wt) => wt.File($"./logs/log-{acc}-.txt",
                                                                        rollingInterval: RollingInterval.Day,
                                                                        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
              .CreateLogger();
        }

        public LinkedList<LogMessage> GetLog(int accountId)
        {
            if (_logs.TryGetValue(accountId, out var logs))
            {
                lock (_objLocks[accountId])
                {
                    return new LinkedList<LogMessage>(logs);
                }
            }
            return new LinkedList<LogMessage>();
        }

        private void Add(int accountId, LogMessage log)
        {
            lock (_objLocks[accountId])
            {
                _logs[accountId].AddFirst(log);
                // keeps 200 message
                while (_logs[accountId].Count > 200)
                {
                    _logs[accountId].RemoveLast();
                }

                _eventManager.OnLogUpdated(accountId, log);
            }
        }

        public void AddAccount(int accountId)
        {
            if (_logs.TryAdd(accountId, new LinkedList<LogMessage>()))
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);
                _loggers.Add(accountId, Log.ForContext("Account", account.Username));
                _objLocks.Add(accountId, new());
            }
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }

        public void Information(int accountId, string message)
        {
            Add(accountId, new LogMessage()
            {
                DateTime = DateTime.Now,
                Level = LevelEnum.Information,
                Message = message,
            });
            _loggers[accountId].Information(message);
        }

        public void Warning(int accountId, string message)
        {
            Add(accountId, new LogMessage()
            {
                DateTime = DateTime.Now,
                Level = LevelEnum.Warning,
                Message = message,
            });
            _loggers[accountId].Warning(message);
        }

        public void Error(int accountId, string message, Exception error)
        {
            Add(accountId, new LogMessage()
            {
                DateTime = DateTime.Now,
                Level = LevelEnum.Error,
                Message = $"{message}\n{error}",
            });
            _loggers[accountId].Error(message, error);
        }

        private readonly Dictionary<int, LinkedList<LogMessage>> _logs = new();
        private readonly Dictionary<int, object> _objLocks = new();
        private readonly Dictionary<int, ILogger> _loggers = new();

        private readonly EventManager _eventManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}