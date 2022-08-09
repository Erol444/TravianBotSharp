using MainCore.Enums;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        protected void Retry(string message)
        {
            if (RetryCounter < 4)
            {
                RetryCounter++;
                if (!string.IsNullOrEmpty(message))
                {
                    LogManager.Information(AccountId, $"{message}. Try again. ({RetryCounter} time(s))");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(message))
                {
                    LogManager.Information(AccountId, $"{message}.");
                }
                throw new Exception("Already tries 3 times.");
            }
        }
    }
}