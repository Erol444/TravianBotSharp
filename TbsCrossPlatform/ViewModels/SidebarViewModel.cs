using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using TbsCrossPlatform.Models.UI;

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

        public string UserName { get; set; }
        public string Password { get; set; }

        public SidebarViewModel()
        {
            var active = this.WhenAnyValue(x => x.Account, x => x.Account, (account, hm) => account is not null);

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
                    Id = "1asd",
                    ServerUrl = "ts2.internal",
                    Username = "abc",
                },
                new Account()
                {
                    Id = "20d",
                    ServerUrl = "ts2.internal",
                    Username = "wsd",
                },
                new Account()
                {
                    Id = "3asd",
                    ServerUrl = "ts2.internal",
                    Username = "vca",
                }
            };
        }

        private async Task AddAccountTask()
        {
            await Task.Yield();
            // Code for executing the command here.
        }

        private async Task AddAccountsTask()
        {
            await Task.Yield();
            // Code for executing the command here.
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