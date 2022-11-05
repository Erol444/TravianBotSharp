using ReactiveUI;

namespace UI.ViewModels.UserControls
{
    public class ToleranceViewModel : ReactiveObject
    {
        public ToleranceViewModel(string text, string unit, bool allowNegative = false) : base()
        {
            Text = text;
            Unit = unit;
            Min = allowNegative ? int.MinValue : 0;
        }

        private int _min;

        public int Min
        {
            get => _min;
            set => this.RaiseAndSetIfChanged(ref _min, value);
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, $"{value}: ");
        }

        private string _unit;

        public string Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        private int _value;

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private int _tolerance;

        public int Tolerance
        {
            get => _tolerance;
            set => this.RaiseAndSetIfChanged(ref _tolerance, value);
        }
    }
}