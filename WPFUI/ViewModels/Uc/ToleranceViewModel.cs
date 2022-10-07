using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class ToleranceViewModel : ReactiveObject
    {
        public ToleranceViewModel(string text, string unit) : base()
        {
            Text = text;
            Unit = unit;
        }

        private string _text;

        public string Text
        {
            get => $"{_text}: ";
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private string _unit;

        public string Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        private string _mainValue;

        public string MainValue
        {
            get => _mainValue;
            set => this.RaiseAndSetIfChanged(ref _mainValue, value);
        }

        private string _toleranceValue;

        public string ToleranceValue
        {
            get => _toleranceValue;
            set => this.RaiseAndSetIfChanged(ref _toleranceValue, value);
        }
    }
}