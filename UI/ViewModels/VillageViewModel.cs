using ReactiveUI;
using System;
using System.Reactive.Linq;
using UI.Models;

namespace UI.ViewModels
{
    public class VillageViewModel : ViewModelBase
    {
        public VillageViewModel()
        {
            this.WhenAnyValue(vm => vm.Village).Subscribe(x =>
            {
                if (x is null) return;

                OnVillageChanged(x.Id);
            });

            this.WhenAnyValue(vm => vm.Village).Select(x => x is not null).ToProperty(this, vm => vm.IsVillageSelected, out _isVillageSelected);
            this.WhenAnyValue(vm => vm.IsVillageSelected).Select(x => !x).ToProperty(this, vm => vm.IsVillageNotSelected, out _isVillageNotSelected);
        }

        public event Action<int> VillageChanged;

        private void OnVillageChanged(int village) => VillageChanged?.Invoke(village);

        private VillageModel _village;

        public VillageModel Village
        {
            get => _village;
            set => this.RaiseAndSetIfChanged(ref _village, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isVillageSelected;

        public bool IsVillageSelected
        {
            get => _isVillageSelected.Value;
        }

        private readonly ObservableAsPropertyHelper<bool> _isVillageNotSelected;

        public bool IsVillageNotSelected
        {
            get => _isVillageNotSelected.Value;
        }
    }
}