using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class FarmListControllerViewModel : TabBaseViewModel
    {
        public FarmListControllerViewModel() : base()
        {
            this.WhenAnyValue(x => x.CurrentFarm).Subscribe(LoadData);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveData, this.WhenAnyValue(x => x.IsActive));
        }

        public void LoadData(FarmInfo farm)
        {
            if (farm is null)
            {
                FarmName = "Not selected";
                FarmCount = "~";
                IsActive = true;
                FarmSetting.IsActive = false;
                FarmSetting.IntervalTime = "0";
                FarmSetting.IntervalDiffTime = "0";
            }
            else
            {
                FarmName = farm.Name;
                using var context = _contextFactory.CreateDbContext();
                var count = context.Farms.Find(farm.Id).FarmCount;
                FarmCount = count.ToString();
                var setting = context.FarmsSettings.Find(farm.Id);
                FarmSetting.CopyFrom(setting);
            }
        }

        public async Task SaveData()
        {
            if (CurrentFarm is null) return;
            _waitingWindow.ViewModel.Show("Saving ...");
            await Task.Run(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.FarmsSettings.Find(CurrentFarm.Id);
                FarmSetting.CopyTo(setting);
                context.Update(setting);
                context.SaveChanges();
            });
            _waitingWindow.ViewModel.Close();
            MessageBox.Show("Saved");
        }

        private FarmInfo _currentFarm;

        public FarmInfo CurrentFarm
        {
            get => _currentFarm;
            set => this.RaiseAndSetIfChanged(ref _currentFarm, value);
        }

        private string _farmName;

        public string FarmName
        {
            get => _farmName;
            set => this.RaiseAndSetIfChanged(ref _farmName, value);
        }

        private string _farmCount;

        public string FarmCount
        {
            get => _farmCount;
            set => this.RaiseAndSetIfChanged(ref _farmCount, value);
        }

        public FarmSettingInfo FarmSetting { get; } = new();

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    }
}