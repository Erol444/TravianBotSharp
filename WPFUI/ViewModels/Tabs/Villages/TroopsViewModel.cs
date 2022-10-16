using MainCore.Enums;
using MainCore.Tasks.Misc;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class TroopsViewModel : VillageTabBaseViewModel, ITabPage
    {
        public TroopsViewModel() : base()
        {
            ApplyCommand = ReactiveCommand.Create(ApplyTask);
        }

        protected override void LoadData(int index)
        {
            CurrentLevel.Clear();
            for (var i = TroopEnums.Legionnaire; i < TroopEnums.RomanSettler; i++)
            {
                CurrentLevel.Add(new TroopInfo
                {
                    Troop = i,
                    Num = "0"
                });
            }

            WantLevel.Clear();
            for (var i = TroopEnums.Legionnaire; i < TroopEnums.RomanSettler; i++)
            {
                WantLevel.Add(new TroopInfo
                {
                    Troop = i,
                    Num = "0"
                });
            }
        }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentVillage is null) return;
            LoadData(CurrentVillage.Id);
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        private void ApplyTask()
        {
            foreach (var item in WantLevel)
            {
                if (item.Num != "0")
                {
                    _taskManager.Add(CurrentAccount.Id, new ImproveTroopsTask(item.Troop, CurrentVillage.Id, CurrentAccount.Id));
                    MessageBox.Show("Apply");
                    return;
                }
            }
            MessageBox.Show("NO troop selected");
        }

        public ObservableCollection<TroopInfo> CurrentLevel { get; } = new();
        public ObservableCollection<TroopInfo> WantLevel { get; } = new();
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }

        public bool IsActive { get; set; }
    }
}