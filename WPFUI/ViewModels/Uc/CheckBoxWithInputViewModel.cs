using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class CheckBoxWithInputViewModel : ReactiveObject
    {
        public CheckBoxWithInputViewModel() : base()
        {
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