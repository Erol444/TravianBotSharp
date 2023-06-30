using DynamicData;
using DynamicData.Kernel;
using MainCore;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : AccountTabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly VillageNavigationStore _villageNavigationStore;
        public ObservableCollection<TabHeaderViewModel> TabHeaders { get; } = new();
        public List<ActivatableViewModelBase> TabContents { get; }

        public ObservableCollection<ListBoxItem> Villages { get; } = new();

        private ListBoxItem _currentVillage;

        public VillagesViewModel(SelectedItemStore SelectedItemStore, IDbContextFactory<AppDbContext> contextFactory, VillageNavigationStore villageNavigationStore) : base(SelectedItemStore)
        {
            _villageNavigationStore = villageNavigationStore;
            _contextFactory = contextFactory;

            _villageNavigationStore.OnTabChanged += OnTabChanged;
            TabContents = _villageNavigationStore.ViewModelList;

            this.WhenAnyValue(vm => vm.CurrentVillage).BindTo(_selectedItemStore, vm => vm.Village);
        }

        private void OnTabChanged(TabHeaderViewModel[] tabHeaders)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                TabHeaders.Clear();
                TabHeaders.AddRange(tabHeaders);
            });
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages
                .Where(x => x.AccountId == accountId)
                .OrderBy(x => x.Name)
                .AsList()
                .Select(x => new ListBoxItem(x.Id, x.Name, x.X, x.Y))
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Villages.Clear();
                Villages.AddRange(villages);
                if (villages.Any())
                {
                    CurrentVillage = Villages.First();
                    _villageNavigationStore.SetTab(TabType.Normal);
                }
                else
                {
                    CurrentVillage = null;
                    _villageNavigationStore.SetTab(TabType.NoAccount);
                }
            });
        }

        public ListBoxItem CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }
    }
}