using MainCore.Enums;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace MainCore.Tasks
{
    public abstract class BotTask
    {
        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }
        public int RetryCounter { get; set; }
        protected bool StopFlag { get; set; }
        public CancellationTokenSource Cts { get; set; }

        protected IDbContextFactory<AppDbContext> _contextFactory;
        protected IChromeBrowser _chromeBrowser;
        protected ITaskManager _taskManager;
        protected EventManager _eventManager;
        protected ILogManager _logManager;
        protected IPlanManager _planManager;
        protected IRestClientManager _restClientManager;

        public string Name { protected set; get; }

        public abstract void Execute();

        public virtual void CopyFrom(BotTask source)
        {
            _contextFactory = source._contextFactory;
            _eventManager = source._eventManager;
            _taskManager = source._taskManager;
            _logManager = source._logManager;
            _chromeBrowser = source._chromeBrowser;
            _planManager = source._planManager;
            _restClientManager = source._restClientManager;
            Cts = source.Cts;
        }

        public virtual void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, EventManager EventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            _contextFactory = contextFactory;
            _eventManager = EventManager;
            _taskManager = taskManager;
            _logManager = logManager;
            _chromeBrowser = chromeBrowser;
            _planManager = planManager;
            _restClientManager = restClientManager;
        }

        public abstract void Refresh();
    }
}