using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using TbsCrossPlatform.Models.UI;
using TbsCrossPlatform.Views;

namespace TbsCrossPlatform.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Account> _accounts;
        private Account _account = null;
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
        }

        public Account Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        public SidebarViewModel()
        {
            var active = this.WhenAnyValue(x => x.Account, x => x.Account, (x, y) => x is not null);

            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask, active);
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, active);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, active);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, active);
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);

            _accounts = new()
            {
                new Account()
                {
                    Id = 1,
                    ServerUrl = "ts2.internal",
                    Username = "abc",
                },
                new Account()
                {
                    Id = 2,
                    ServerUrl = "ts2.internal",
                    Username = "wsd",
                },
                new Account()
                {
                    Id = 3,
                    ServerUrl = "ts2.internal",
                    Username = "vca",
                }
            };
        }

        private async Task AddAccountsTask()
        {
            await Task.Yield();
            var window = new AddAccountsWindow();
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var result = await window.ShowDialog<List<AccountInput>>(desktop.MainWindow);
                foreach (var item in result)
                {
                    _accounts.Add(new Account { Id = item.Id, ServerUrl = item.ServerUrl, Username = item.Username });
                }
            }
        }

        private async Task LoginTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }

        private async Task LogoutTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }

        private async Task LoginAllTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }

        private async Task LogoutAllTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }

        private async Task EditAccountTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }

        private async Task DeleteAccountTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }
    }
}