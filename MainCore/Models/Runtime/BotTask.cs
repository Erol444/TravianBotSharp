using MainCore.Enums;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MainCore.Models.Runtime
{
    public abstract class BotTask
    {
        protected IDbContextFactory<AppDbContext> _contextFactory;
        protected IChromeBrowser _chromeBrowser;
        protected ITaskManager _taskManager;
        protected IDatabaseEvent _databaseEvent;
        protected ILogManager _logManager;

        protected int _accountId;

        public BotTask(int accountId, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IDatabaseEvent databaseEvent, ILogManager logManager)
        {
            _accountId = accountId;

            _contextFactory = contextFactory;
            _chromeBrowser = chromeBrowser;
            _taskManager = taskManager;
            _databaseEvent = databaseEvent;
            _logManager = logManager;
        }

        public TaskStage Stage { get; protected set; }
        public DateTime ExecuteAt { get; protected set; }
        protected int RetryCounter { get; set; } = 0;

        public abstract Task<TaskRes> Execute();
    }
}