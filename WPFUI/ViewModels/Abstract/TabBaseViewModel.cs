using MainCore;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class TabBaseViewModel : ReactiveObject
    {
        public TabBaseViewModel()
        {
            _contextFactory = Locator.Current.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = Locator.Current.GetService<IEventManager>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
            _planManager = Locator.Current.GetService<IPlanManager>();
            _waitingWindow = Locator.Current.GetService<WaitingViewModel>();
            _useragentManager = Locator.Current.GetService<IUseragentManager>();
            _restClientManager = Locator.Current.GetService<IRestClientManager>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
            _logManager = Locator.Current.GetService<ILogManager>();
        }

        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IEventManager _eventManager;
        protected readonly ITaskManager _taskManager;
        protected readonly IPlanManager _planManager;
        protected readonly IUseragentManager _useragentManager;
        protected readonly IRestClientManager _restClientManager;
        protected readonly ILogManager _logManager;
        protected readonly WaitingViewModel _waitingWindow;
    }
}