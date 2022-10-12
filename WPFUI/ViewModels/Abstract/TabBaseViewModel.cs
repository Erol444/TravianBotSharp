using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using WPFUI.Views;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class TabBaseViewModel : ReactiveObject
    {
        public TabBaseViewModel()
        {
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _eventManager = App.GetService<EventManager>();
            _taskManager = App.GetService<ITaskManager>();
            _planManager = App.GetService<IPlanManager>();
            _waitingWindow = App.GetService<WaitingWindow>();
            _useragentManager = App.GetService<IUseragentManager>();
            _restClientManager = App.GetService<IRestClientManager>();
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
        }

        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly EventManager _eventManager;
        protected readonly ITaskManager _taskManager;
        protected readonly IPlanManager _planManager;
        protected readonly IUseragentManager _useragentManager;
        protected readonly IRestClientManager _restClientManager;
        protected readonly ILogManager _logManager;
        protected readonly WaitingWindow _waitingWindow;
    }
}