using MainCore.Enums;
using MainCore.Helper;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Update;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
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
            UpdateCommand = ReactiveCommand.Create(UpdateTask);
            _eventManager.TroopLevelUpdate += OnTroopLevelUpdate;
        }

        private void OnTroopLevelUpdate(int villageId)
        {
            if (CurrentVillage is null) return;
            if (villageId != CurrentVillage.Id) return;
            RxApp.MainThreadScheduler.Schedule(() => LoadData(villageId));
        }

        protected override void LoadData(int index)
        {
            LoadCurrent(index);
            LoadWant(index);
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

        private void LoadCurrent(int villageId)
        {
            CurrentLevel.Clear();
            using var context = _contextFactory.CreateDbContext();
            var troops = context.VillagesTroops.Where(x => x.VillageId == villageId).ToArray();
            for (var i = 0; i < troops.Length; i++)
            {
                var troop = troops[i];
                CurrentLevel.Add(new TroopInfoText
                {
                    Troop = (TroopEnums)troop.Id,
                    Text = troop.Level,
                });
            }
        }

        private void LoadWant(int villageId)
        {
            WantUpgrade.Clear();
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(villageId);
            var boolean = settings.GetTroopUpgrade();
            var tribe = context.AccountsInfo.Find(CurrentAccount.Id).Tribe;
            var troops = tribe.GetTroops();
            for (var i = 0; i < troops.Count; i++)
            {
                var troop = troops[i];
                WantUpgrade.Add(new TroopInfoCheckBox
                {
                    Troop = troop,
                    IsChecked = boolean[i],
                });
            }
        }

        private void ApplyTask()
        {
            foreach (var item in WantUpgrade)
            {
                if (item.IsChecked)
                {
                    _taskManager.Add(CurrentAccount.Id, new ImproveTroopsTask(CurrentVillage.Id, CurrentAccount.Id));
                    MessageBox.Show("Apply");
                    return;
                }
            }
            MessageBox.Show("NO troop selected");
        }

        private void UpdateTask()
        {
            _taskManager.Add(CurrentAccount.Id, new UpdateTroopLevel(CurrentVillage.Id, CurrentAccount.Id));
            MessageBox.Show("Update");
        }

        public ObservableCollection<TroopInfoText> CurrentLevel { get; } = new();
        public ObservableCollection<TroopInfoCheckBox> WantUpgrade { get; } = new();
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }

        public bool IsActive { get; set; }
    }
}