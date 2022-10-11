using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class CheckBoxWithInputViewModel : ReactiveObject
    {
        public CheckBoxWithInputViewModel(string text, string unit) : base()
        {
            Text = text;
            Unit = unit;
        }

        private string _text;

        public string Text
        {
            get => $"{_text} ";
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        private string _value;

        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private string _unit;

        public string Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }
    }
}