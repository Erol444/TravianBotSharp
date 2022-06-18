using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace WPFUI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            AddAccountsCommand = ReactiveCommand.CreateFromTask(AddAccountsTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);
            DeleteAccountCommand = ReactiveCommand.CreateFromTask(DeleteAccountTask);
            LoginCommand = ReactiveCommand.CreateFromTask(LoginTask);
            LogoutCommand = ReactiveCommand.CreateFromTask(LogoutTask);
            LoginAllCommand = ReactiveCommand.CreateFromTask(LoginAllTask);
            LogoutAllCommand = ReactiveCommand.CreateFromTask(LogoutAllTask);
        }

        private async Task AddAccountTask()
        {
            await Task.Yield();
        }

        private async Task AddAccountsTask()
        {
            await Task.Yield();
        }

        private async Task LoginTask()
        {
            await Task.Yield();
        }

        private async Task LogoutTask()
        {
            await Task.Yield();
        }

        private async Task LoginAllTask()
        {
            await Task.Yield();
        }

        private async Task LogoutAllTask()
        {
            await Task.Yield();
        }

        private async Task EditAccountTask()
        {
            await Task.Yield();
        }

        private async Task DeleteAccountTask()
        {
            await Task.Yield();
        }

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