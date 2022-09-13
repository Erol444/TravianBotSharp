using MainCore.Models.Database;
using ReactiveUI;

namespace WPFUI.Models
{
    public class FarmSettingInfo : ReactiveObject
    {
        public void CopyFrom(FarmSetting settings)
        {
            IsActive = settings.IsActive;
            IntervalTime = $"{(settings.IntervalMax + settings.IntervalMin) / 2}";
            IntervalDiffTime = $"{(settings.IntervalMax - settings.IntervalMin) / 2}";
        }

        public void CopyTo(FarmSetting settings)
        {
            settings.IsActive = IsActive;

            var intervalTime = int.Parse(IntervalTime);
            var intervalDiffTime = int.Parse(IntervalDiffTime);
            settings.IntervalMin = intervalTime - intervalDiffTime;
            settings.IntervalMax = intervalTime + intervalDiffTime;
        }

        public void Clear()
        {
            IsActive = false;
            IntervalTime = "~";
            IntervalDiffTime = "~";
        }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        private string _intervalTime;

        public string IntervalTime
        {
            get => _intervalTime;
            set => this.RaiseAndSetIfChanged(ref _intervalTime, value);
        }

        private string _intervalDiffTime;

        public string IntervalDiffTime
        {
            get => _intervalDiffTime;
            set => this.RaiseAndSetIfChanged(ref _intervalDiffTime, value);
        }
    }
}