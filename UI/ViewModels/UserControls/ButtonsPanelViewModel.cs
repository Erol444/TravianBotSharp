using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace UI.ViewModels.UserControls
{
    public class ButtonsPanelViewModel : ViewModelBase
    {
        public ButtonsPanelViewModel() : base()
        {
            CheckVersionCommand = ReactiveCommand.CreateFromTask(CheckVersionTask);
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask, this.WhenAnyValue(vm => vm.IsAccountSelected));
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm.IsAccountSelected, (a, b) => a && b));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask, this.WhenAnyValue(vm => vm.IsAllowLogin, vm => vm.IsAccountSelected, (a, b) => a && b));
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask, this.WhenAnyValue(vm => vm.IsAllowLogout, vm => vm.IsAccountSelected, (a, b) => a && b));
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);
        }

        private Task CheckVersionTask()
        {
            return Task.CompletedTask;
        }

        private Task AddAccountTask()
        {
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

        public ReactiveCommand<Unit, Unit> CheckVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAccountsCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginAllCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutAllCommand { get; }
    }
}