using Avalonia.Threading;
using MainCore;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using UI.Models;

namespace UI.ViewModels.UserControls
{
    public class VillagesTableViewModel : ViewModelBase
    {
        public VillagesTableViewModel(IDbContextFactory<AppDbContext> contextFactory, LoadingOverlayViewModel loadingOverlayViewModel, AccountViewModel accountViewModel, VillageViewModel villageViewModel)
        {
            _contextFactory = contextFactory;
            _loadingOverlayViewModel = loadingOverlayViewModel;
            _accountViewModel = accountViewModel;
            _villageViewModel = villageViewModel;
            this.WhenAnyValue(vm => vm.CurrentVillage).Subscribe(x => _villageViewModel.Village = x);
            _accountViewModel.AccountChanged += OnAccountChanged;
        }

        private void OnAccountChanged(int accountId)
        {
            RxApp.MainThreadScheduler.Schedule(() => Load(_accountViewModel.Account.Id));
        }

        public async Task LoadTask()
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Loading villages ...";

            await Dispatcher.UIThread.InvokeAsync(() => Load(_accountViewModel.Account.Id));

            _loadingOverlayViewModel.Unload();
        }

        private void Load(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == accountId);

            Villages.Clear();
            if (villages.Any())
            {
                foreach (var village in villages)
                {
                    Villages.Add(new()
                    {
                        Id = village.Id,
                        Name = village.Name,
                        Coords = $"{village.X}|{village.Y}",
                    });
                }
            }
        }

        private VillageModel _currentVillage;

        public VillageModel CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set => this.RaiseAndSetIfChanged(ref _currentIndex, value);
        }

        public ObservableCollection<VillageModel> Villages { get; } = new();

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;
        private readonly AccountViewModel _accountViewModel;
        private readonly VillageViewModel _villageViewModel;
    }
}