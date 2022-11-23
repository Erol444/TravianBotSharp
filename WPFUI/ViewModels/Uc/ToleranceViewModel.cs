using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Uc
{
    public class ToleranceViewModel : ReactiveObject
    {
        public ToleranceViewModel(string text, string unit) : base()
        {
            Text = text;
            Unit = unit;
            this.WhenAnyValue(vm => vm.MainValue).Subscribe(x =>
            {
                ToleranceMax = x;
                if (ToleranceValue > x) ToleranceValue = x;
            });
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

        private int _mainValue;

        public int MainValue
        {
            get => _mainValue;
            set => this.RaiseAndSetIfChanged(ref _mainValue, value);
        }

        private int _toleranceValue;

        public int ToleranceValue
        {
            get => _toleranceValue;
            set => this.RaiseAndSetIfChanged(ref _toleranceValue, value);
        }

        private int _toleranceMax;

        public int ToleranceMax
        {
            get => _toleranceMax;
            set => this.RaiseAndSetIfChanged(ref _toleranceMax, value);
        }
    }
}