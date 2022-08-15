using MainCore.Enums;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;

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
        protected bool StopFlag { get; set; }

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
            source.ContextFactory = destination.ContextFactory;
            source.DatabaseEvent = destination.DatabaseEvent;
            source.TaskManager = destination.TaskManager;
            source.LogManager = destination.LogManager;
            source.ChromeBrowser = destination.ChromeBrowser;
            source.PlanManager = destination.PlanManager;
        }
    }
}