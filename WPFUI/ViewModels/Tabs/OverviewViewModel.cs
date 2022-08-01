using MainCore.Models.Database;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using WPFUI.Interfaces;

namespace WPFUI.ViewModels.Tabs
{
    public class OverviewViewModel : ReactiveObject, IMainTabPage
    {
        public OverviewViewModel()
        {
            SaveCommand = ReactiveCommand.Create(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask, this.WhenAnyValue(vm => vm.IsSelected));
            ImportCommand = ReactiveCommand.Create(ImportTask, this.WhenAnyValue(vm => vm.IsSelected));
        }

        public void OnActived()
        {
        }

        public void SaveTask()
        {
        }

        public void ExportTask()
        {
        }

        public void ImportTask()
        {
        }

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