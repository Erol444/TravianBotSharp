using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc
{
    public class TroopTrainingSelectorViewModel : ReactiveObject
    {
        public TroopTrainingSelectorViewModel()
        {
            Troops = new();
            FillTime = new();
        }

        public void LoadData(IEnumerable<TroopInfo> troops)
        {
            Troops.Clear();
            Troops.AddRange(troops);
        }

        public ObservableCollection<TroopInfo> Troops { get; }
        private TroopInfo _selectedItem;

        public TroopInfo SelectedTroop
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ToleranceViewModel FillTime { get; }

        private bool _isGreat;

        public bool IsGreat
        {
            get => _isGreat;
            set => this.RaiseAndSetIfChanged(ref _isGreat, value);
        }
    }
}