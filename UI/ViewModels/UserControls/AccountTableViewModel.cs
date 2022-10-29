using Avalonia.Threading;
using MainCore;
using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace UI.ViewModels.UserControls
{
    public class AccountTableViewModel : ViewModelBase
    {
        public AccountTableViewModel(IDbContextFactory<AppDbContext> contextFactory, LoadingOverlayViewModel loadingOverlayViewModel) : base()
        {
            _contextFactory = contextFactory;
            _loadingOverlayViewModel = loadingOverlayViewModel;

            LoadCommand = ReactiveCommand.CreateFromTask(LoadTask);
        }

        protected override void OnActived()
        {
            //LoadCommand.Execute().Subscribe();
        }

        public Task LoadData() => LoadTask();

        private async Task LoadTask()
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Loading accounts ...";

            await Dispatcher.UIThread.InvokeAsync(Load);

            _loadingOverlayViewModel.Unload();
        }

        private void Load()
        {
            using var context = _contextFactory.CreateDbContext();
            Accounts.Clear();
            if (context.Accounts.Any())
            {
                foreach (var item in context.Accounts)
                {
                    Accounts.Add(item);
                }
            }
        }

        public ObservableCollection<Account> Accounts { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
    }
}