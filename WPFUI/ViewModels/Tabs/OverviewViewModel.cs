using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using WPFUI.Interfaces;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class OverviewViewModel : ReactiveObject, IMainTabPage
    {
        public OverviewViewModel()
        {
            _eventManager = App.GetService<IEventManager>();
            _eventManager.VillagesUpdated += OnVillagesUpdated;
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask, this.WhenAnyValue(vm => vm.IsSelected));
            ImportCommand = ReactiveCommand.Create(ImportTask, this.WhenAnyValue(vm => vm.IsSelected));
        }

        private void OnVillagesUpdated(int accountId)
        {
            if (AccountId != accountId) return;
            App.Current.Dispatcher.Invoke(LoadData, accountId);
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public void LoadData(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == accountId);
            VillagesSettings.Clear();
            foreach (var village in villages)
            {
                var setting = context.VillagesSettings.Find(village.Id);
                VillagesSettings.Add(new()
                {
                    Id = setting.VillageId,
                    Name = village.Name,
                    Coords = $"{village.X},{village.Y}",
                    IsUseHeroRes = setting.IsUseHeroRes,
                    IsInstantComplete = setting.IsInstantComplete,
                    InstantCompleteTime = setting.InstantCompleteTime
                });
            }
        }

        public async Task SaveTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            foreach (var village in VillagesSettings)
            {
                var setting = context.VillagesSettings.Find(village.Id);
                setting.IsUseHeroRes = village.IsUseHeroRes;
                setting.IsInstantComplete = village.IsInstantComplete;
                setting.InstantCompleteTime = village.InstantCompleteTime;
                context.Update(setting);
            }
            await context.SaveChangesAsync();
        }

        public void ExportTask()
        {
        }

        public void ImportTask()
        {
        }

        private readonly IEventManager _eventManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public ReactiveCommand<Unit, Unit> SaveCommand;
        public ReactiveCommand<Unit, Unit> ExportCommand;
        public ReactiveCommand<Unit, Unit> ImportCommand;
        public ObservableCollection<VillageSetting> VillagesSettings { get; } = new();
        private VillageSetting _current;

        public VillageSetting Current
        {
            get => _current;
            set
            {
                this.RaiseAndSetIfChanged(ref _current, value);
                if (value is not null) IsSelected = true;
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }
    }
}