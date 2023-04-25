using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Uc
{
    public class ToleranceViewModel : ReactiveObject
    {
        public ToleranceViewModel() : base()
        {
            this.WhenAnyValue(vm => vm.MainValue).Subscribe(x =>
            {
                ToleranceMax = x;
                if (ToleranceValue > x) ToleranceValue = x;
            });
        }

        public (int, int) GetTolerance()
        {
            return (MainValue, ToleranceValue);
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