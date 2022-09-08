using MainCore.Enums;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace MainCore.Tasks
{
    public abstract class BotTask
    {
        private readonly int _accountId;
        public int AccountId => _accountId;

        public BotTask(int accountId)
        {
            _accountId = accountId;
        }

        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }
        public int RetryCounter { get; set; }
        public CancellationTokenSource Cts { get; set; }

        public IDbContextFactory<AppDbContext> ContextFactory { get; set; }
        public IChromeBrowser ChromeBrowser { get; set; }
        public ITaskManager TaskManager { get; set; }
        public IEventManager DatabaseEvent { get; set; }
        public ILogManager LogManager { get; set; }
        public IPlanManager PlanManager { get; set; }
        public IRestClientManager RestClientManager { get; set; }

        public abstract string Name { get; }

        public abstract void Execute();

        public virtual void CopyFrom(BotTask source)
        {
            ContextFactory = source.ContextFactory;
            DatabaseEvent = source.DatabaseEvent;
            TaskManager = source.TaskManager;
            LogManager = source.LogManager;
            ChromeBrowser = source.ChromeBrowser;
            PlanManager = source.PlanManager;
            RestClientManager = source.RestClientManager;
            Cts = source.Cts;
        }
    }
}