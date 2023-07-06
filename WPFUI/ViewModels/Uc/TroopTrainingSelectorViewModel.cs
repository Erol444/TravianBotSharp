﻿using DynamicData;
using MainCore.Enums;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc
{
    public class TroopTrainingSelectorViewModel : ReactiveObject
    {
        public void LoadData(IEnumerable<TroopInfo> troops, TroopEnums selectedTroop, int min, int max, bool isGreat)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Troops.Clear();
                Troops.Add(new(TroopEnums.None));
                Troops.AddRange(troops);
                SelectedTroop = Troops.FirstOrDefault(x => x.Troop == selectedTroop) ?? Troops.First();

                FillTime.LoadData(min, max);
                IsGreat = isGreat;
            });
        }

        public (TroopEnums, int, int, bool) GetData()
        {
            var (min, max) = FillTime.GetData();
            return (SelectedTroop.Troop, min, max, IsGreat);
        }

        public ObservableCollection<TroopInfo> Troops { get; } = new();
        private TroopInfo _selectedItem;

        public TroopInfo SelectedTroop
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ToleranceViewModel FillTime { get; } = new();

        private bool _isGreat;

        public bool IsGreat
        {
            get => _isGreat;
            set => this.RaiseAndSetIfChanged(ref _isGreat, value);
        }
    }
}