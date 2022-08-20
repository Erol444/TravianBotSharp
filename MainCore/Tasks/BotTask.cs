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
        protected int RetryCounter { get; set; }
        public CancellationTokenSource Cts { get; set; }

        public IDbContextFactory<AppDbContext> ContextFactory { get; set; }
        public IChromeBrowser ChromeBrowser { get; set; }
        public ITaskManager TaskManager { get; set; }
        public IEventManager DatabaseEvent { get; set; }
        public ILogManager LogManager { get; set; }
        public IPlanManager PlanManager { get; set; }

        public abstract string Name { get; }

        public abstract void Execute();
    }

    public static class BotTaskExtension
    {
        public static void CopyTo(this BotTask source, BotTask destination)
        {
            destination.ContextFactory = source.ContextFactory;
            destination.DatabaseEvent = source.DatabaseEvent;
            destination.TaskManager = source.TaskManager;
            destination.LogManager = source.LogManager;
            destination.ChromeBrowser = source.ChromeBrowser;
            destination.PlanManager = source.PlanManager;
            destination.Cts = source.Cts;
        }
    }
}