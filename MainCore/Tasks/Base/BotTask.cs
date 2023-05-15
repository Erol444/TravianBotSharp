using FluentResults;
using MainCore.Enums;
using Microsoft.EntityFrameworkCore;
using Splat;
using System;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class BotTask
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        public BotTask(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;

            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
        }

        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }

        protected string _name;

        public CancellationToken CancellationToken { get; set; }

        public abstract string GetName();

        public abstract Result Execute();
    }
}