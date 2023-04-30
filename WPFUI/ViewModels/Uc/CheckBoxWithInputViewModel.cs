using ReactiveUI;
using System.Reactive.Concurrency;

namespace WPFUI.ViewModels.Uc
{
    public class CheckBoxWithInputViewModel : ReactiveObject
    {
        public void LoadData(bool isChecked, int value)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                IsChecked = isChecked;
                Value = value;
            });
        }

        public (bool, int) GetData()
        {
            return (IsChecked, Value);
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        private int _value;

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}