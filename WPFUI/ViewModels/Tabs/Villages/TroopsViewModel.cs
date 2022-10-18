using MainCore.Enums;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Update;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
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
            UpdateCommand = ReactiveCommand.Create(UpdateTask);
        }

        protected override void LoadData(int index)
        {
            CurrentLevel.Clear();
            WantLevel.Clear();
            using var context = _contextFactory.CreateDbContext();
            var troops = context.VillagesTroops.Where(x => x.VillageId == CurrentVillage.Id).ToArray();
            for (var i = 0; i < troops.Length; i++)
            {
                var troop = troops[i];
                CurrentLevel.Add(new TroopInfo
                {
                    Troop = (TroopEnums)troop.Id,
                    Num = troop.Level.ToString()
                });
                WantLevel.Add(new TroopInfo
                {
                    Troop = (TroopEnums)troop.Id,
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

        private void UpdateTask()
        {
            _taskManager.Add(CurrentAccount.Id, new UpdateTroopLevel(CurrentVillage.Id, CurrentAccount.Id));
            MessageBox.Show("Update");
        }

        public ObservableCollection<TroopInfo> CurrentLevel { get; } = new();
        public ObservableCollection<TroopInfo> WantLevel { get; } = new();
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }

        public bool IsActive { get; set; }
    }
}