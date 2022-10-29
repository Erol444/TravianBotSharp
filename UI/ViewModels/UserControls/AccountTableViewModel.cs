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
        public AccountTableViewModel(IDbContextFactory<AppDbContext> contextFactory) : base()
        {
            _contextFactory = contextFactory;

            LoadCommand = ReactiveCommand.CreateFromTask(LoadTask);
        }

        protected override void OnActived()
        {
            //LoadCommand.Execute().Subscribe();
        }

        private async Task LoadTask()
        {
            IsLoading = true;
            await Task.Delay(10000);
            using var context = _contextFactory.CreateDbContext();
            Accounts.Clear();

            if (context.Accounts.Any())
            {
                foreach (var item in context.Accounts)
                {
                    Accounts.Add(item);
                }
            }
            IsLoading = false;
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public ObservableCollection<Account> Accounts { get; } = new();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}