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
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class VillageTroopsViewModel : VillageTabViewModelBase
    {
        public VillageTroopsViewModel()
        {
            ApplyCommand = ReactiveCommand.Create(ApplyTask);
            UpdateCommand = ReactiveCommand.Create(UpdateTask);
            _eventManager.TroopLevelUpdate += OnTroopLevelUpdate;
        }

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void OnTroopLevelUpdate(int villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            LoadData(villageId);
        }

        private void LoadData(int index)
        {
            LoadCurrent(index);
            LoadWant(index);
        }

        private void LoadCurrent(int villageId)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
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
            });
        }

        private void LoadWant(int villageId)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                WantUpgrade.Clear();
                using var context = _contextFactory.CreateDbContext();
                var settings = context.VillagesSettings.Find(villageId);
                var boolean = settings.GetTroopUpgrade();
                var tribe = context.AccountsInfo.Find(AccountId).Tribe;
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
            });
        }

        private void ApplyTask()
        {
            foreach (var item in WantUpgrade)
            {
                if (item.IsChecked)
                {
                    _taskManager.Add(AccountId, new ImproveTroopsTask(VillageId, AccountId));
                    MessageBox.Show("Apply");
                    return;
                }
            }
            MessageBox.Show("NO troop selected");
        }

        private void UpdateTask()
        {
            _taskManager.Add(AccountId, new UpdateTroopLevel(VillageId, AccountId));
            MessageBox.Show("Update");
        }

        public ObservableCollection<TroopInfoText> CurrentLevel { get; } = new();
        public ObservableCollection<TroopInfoCheckBox> WantUpgrade { get; } = new();
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
    }
}