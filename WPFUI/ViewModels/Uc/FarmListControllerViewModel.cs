using DynamicData.Binding;
using ReactiveUI;
using System;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class FarmListControllerViewModel : TabBaseViewModel
    {
        public FarmListControllerViewModel() : base()
        {
            this.WhenAnyValue(x => x.CurrentFarm).Subscribe(LoadData);
            FarmSetting.WhenAnyPropertyChanged().Subscribe(SaveData);
        }

        public void LoadData(FarmInfo farm)
        {
            _isLoading = true;
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
            _isLoading = false;
        }

        public void SaveData(FarmSettingInfo settingInfo)
        {
            if (CurrentFarm is null) return;

            if (_isLoading) return;

            using var context = _contextFactory.CreateDbContext();
            var setting = context.FarmsSettings.Find(CurrentFarm.Id);
            settingInfo.CopyTo(setting);
            context.Update(setting);
            context.SaveChanges();
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

        private bool _isLoading = false;
    }
}