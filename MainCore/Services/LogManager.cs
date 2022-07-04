using MainCore.Enums;
using MainCore.Models.Runtime;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainCore.Services
{
    public sealed class LogManager : ILogManager
    {
        public LogManager(IDatabaseEvent databaseEvent, IDbContextFactory<AppDbContext> contextFactory)
        {
            _databaseEvent = databaseEvent;
            _contextFactory = contextFactory;
        }

        public void Init()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Map("Account", "Other", (acc, wt) => wt.File($"./logs/log-{acc}-.txt",
                                                                        rollingInterval: RollingInterval.Day,
                                                                        encoding: Encoding.Unicode,
                                                                        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
              .CreateLogger();
        }

        public LinkedList<LogMessage> GetLog(int accountId)
        {
            if (_logs.TryGetValue(accountId, out var logs))
            {
                return logs;
            }
            return new LinkedList<LogMessage>();
        }

        private void Add(int accountId, LogMessage log)
        {
            lock (objLock)
            {
                _logs[accountId].AddFirst(log);
                // keeps 200 message
                while (_logs[accountId].Count > 200)
                {
                    _logs[accountId].RemoveLast();
                }

                _databaseEvent.OnLogUpdated(accountId);
            }
        }

        public void AddAccount(int accountId)
        {
            if (_logs.TryAdd(accountId, new LinkedList<LogMessage>()))
            {
                using var context = _contextFactory.CreateDbContext();
                var account = context.Accounts.Find(accountId);
                _loggers.Add(accountId, Log.ForContext("Account", account.Username));
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
            _loggers[accountId].Error(message);
        }

        private readonly object objLock = new();
        private readonly Dictionary<int, LinkedList<LogMessage>> _logs = new();
        private readonly Dictionary<int, ILogger> _loggers = new();

        private readonly IDatabaseEvent _databaseEvent;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}