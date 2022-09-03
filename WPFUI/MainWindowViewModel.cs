using MainCore;
using MainCore.Models.Database;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Views;

namespace WPFUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            _chromeManager = App.GetService<IChromeManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
            _databaseEvent = App.GetService<IEventManager>();
            _taskManager = App.GetService<ITaskManager>();
            _logManager = App.GetService<ILogManager>();
            _timeManager = App.GetService<ITimerManager>();
            _restClientManager = App.GetService<IRestClientManager>();

            _waitingWindow = App.GetService<WaitingWindow>();

            _isAccountNotSelected = this.WhenAnyValue(x => x.IsAccountSelected).Select(x => !x).ToProperty(this, x => x.IsAccountNotSelected);

            ClosingCommand = ReactiveCommand.CreateFromTask<CancelEventArgs>(ClosingTask);
        }

        private async Task ClosingTask(CancelEventArgs e)
        {
            if (_closed) return;
            e.Cancel = true;
            _waitingWindow.ViewModel.Text = "saving data";
            _waitingWindow.Show();
            await Task.Delay(2000);

            var planManager = App.GetService<IPlanManager>();
            planManager.Save();

            var mainWindow = App.GetService<MainWindow>();
            mainWindow.Hide();
            _closed = true;
            _waitingWindow.Close();
            mainWindow.Close();
        }

        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _databaseEvent;
        private readonly ITaskManager _taskManager;
        private readonly ILogManager _logManager;
        private readonly ITimerManager _timeManager;
        private readonly IRestClientManager _restClientManager;

        private readonly WaitingWindow _waitingWindow;

        private bool _closed = false;

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        private bool _isAccountSelected = false;

        public bool IsAccountSelected
        {
            get => _isAccountSelected;
            set => this.RaiseAndSetIfChanged(ref _isAccountSelected, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isAccountNotSelected;

        public bool IsAccountNotSelected
        {
            get => _isAccountNotSelected.Value;
        }

        public ReactiveCommand<CancelEventArgs, Unit> ClosingCommand { get; }
    }
}