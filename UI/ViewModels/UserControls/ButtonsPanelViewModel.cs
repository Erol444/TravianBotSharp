using Avalonia.Threading;
using MainCore.Enums;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels.UserControls
{
    public class ButtonsPanelViewModel : ViewModelBase
    {
        public ButtonsPanelViewModel(AccountViewModel accountViewModel, TabPanelViewModel tabPanelViewModel) : base()
        {
            _accountViewModel = accountViewModel;
            _accountViewModel.StatusChanged += OnStatusChanged;
            _tabPanelViewModel = tabPanelViewModel;

            CheckVersionCommand = ReactiveCommand.CreateFromTask(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask, this.WhenAnyValue(vm => vm._accountViewModel.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._accountViewModel.IsAccountSelected, (a, b) => a && b));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm._accountViewModel.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAllowLogout, vm => vm._accountViewModel.IsAccountSelected, (a, b) => a && b));
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);
        }

        private void OnStatusChanged(AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Offline:
                    IsAllowLogin = true;
                    IsAllowLogout = false;
                    break;

                case AccountStatus.Starting:
                    IsAllowLogin = false;
                    IsAllowLogout = false;
                    break;

                case AccountStatus.Online:
                    IsAllowLogin = false;
                    IsAllowLogout = true;
                    break;

                case AccountStatus.Pausing:
                    IsAllowLogin = false;
                    IsAllowLogout = false;
                    break;

                case AccountStatus.Paused:
                    IsAllowLogin = false;
                    IsAllowLogout = true;
                    break;

                case AccountStatus.Stopping:
                    IsAllowLogin = false;
                    IsAllowLogout = false;
                    break;
            }
        }

        private async Task CheckVersionTask()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var versionWindow = Locator.Current.GetService<VersionWindow>();
                versionWindow.Show();
            });
        }

        private Task AddAccountTask()
        {
            _tabPanelViewModel.SetTab(TabType.AddAccount);
            return Task.CompletedTask;
        }

        private Task AddAccountsTask()
        {
            return Task.CompletedTask;
        }

        private Task LoginTask()
        {
            return Task.CompletedTask;
        }

        private Task LogoutTask()
        {
            return Task.CompletedTask;
        }

        private Task EditAccountTask()
        {
            _tabPanelViewModel.SetTab(TabType.EditAccount);
            return Task.CompletedTask;
        }

        private Task DeleteAccountTask()
        {
            return Task.CompletedTask;
        }

        private Task LogoutAllTask()
        {
            return Task.CompletedTask;
        }

        private Task LoginAllTask()
        {
            return Task.CompletedTask;
        }

        private bool _isAllowLogout;

        public bool IsAllowLogout
        {
            get => _isAllowLogout;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogout, value);
        }

        private bool _isAllowLogin;

        public bool IsAllowLogin
        {
            get => _isAllowLogin;
            set => this.RaiseAndSetIfChanged(ref _isAllowLogin, value);
        }

        public ReactiveCommand<Unit, Unit> CheckVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }

        private readonly AccountViewModel _accountViewModel;
        private readonly TabPanelViewModel _tabPanelViewModel;
    }
}