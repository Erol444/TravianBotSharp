using MainCore.Models.Database;
using ReactiveUI;

namespace WPFUI.Models
{
    public class FarmSettingInfo : ReactiveObject
    {
        public void CopyFrom(FarmSetting settings)
        {
            IsActive = settings.IsActive;
        }

        public void CopyTo(FarmSetting settings)
        {
            settings.IsActive = IsActive;
        }

        public void Clear()
        {
            IsActive = false;
        }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }
    }
}