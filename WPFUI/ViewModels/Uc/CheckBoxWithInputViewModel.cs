using ReactiveUI;
using System.Reactive.Linq;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class CheckBoxWithInputViewModel : ViewModelBase
    {
        public void LoadData(bool isChecked, int value)
        {
            Observable.Start(() =>
            {
                IsChecked = isChecked;
                Value = value;
            }, RxApp.MainThreadScheduler);
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