using MainCore;
using MainCore.Enums;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class VillageTroopsViewModel : VillageTabBaseViewModel
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
            using var context = _contextFactory.CreateDbContext();
            var troops = context.VillagesTroops.Where(x => x.VillageId == villageId).ToArray();
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                CurrentLevel.Clear();
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
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(villageId);
            var boolean = settings.GetTroopUpgrade();
            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            var troops = tribe.GetTroops();
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                WantUpgrade.Clear();
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
                    _taskManager.Add(AccountId, _taskFactory.GetImproveTroopsTask(VillageId, AccountId));
                    MessageBox.Show("Apply");
                    return;
                }
            }
            MessageBox.Show("NO troop selected");
        }

        private void UpdateTask()
        {
            _taskManager.Add(AccountId, _taskFactory.GetUpdateTroopLevelTask(VillageId, AccountId));
            MessageBox.Show("Update");
        }

        public ObservableCollection<TroopInfoText> CurrentLevel { get; } = new();
        public ObservableCollection<TroopInfoCheckBox> WantUpgrade { get; } = new();
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
    }
}