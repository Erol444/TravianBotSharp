using FluentResults;
using MainCore.Enums;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Splat;
using System;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace MainCore.Tasks
{
    public abstract class BotTask
    {
        public BotTask()
        {
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = Locator.Current.GetService<IEventManager>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
            _logManager = Locator.Current.GetService<ILogManager>();
            _chromeManager = Locator.Current.GetService<IChromeManager>();
            _planManager = Locator.Current.GetService<IPlanManager>();
            _restClientManager = Locator.Current.GetService<IRestClientManager>();
        }

        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }

        protected IDbContextFactory<AppDbContext> _contextFactory;
        protected IChromeManager _chromeManager;
        protected IChromeBrowser _chromeBrowser;
        protected ITaskManager _taskManager;
        protected IEventManager _eventManager;
        protected ILogManager _logManager;
        protected IPlanManager _planManager;
        protected IRestClientManager _restClientManager;

        public string Name { protected set; get; }

        public abstract Result Execute();
    }
}