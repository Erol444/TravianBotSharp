using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace WPFUI.ViewModels.Uc
{
    public class ToleranceViewModel : ReactiveObject
    {
        public ToleranceViewModel() : base()
        {
            Observable.ObserveOn(this.WhenAnyValue(vm => vm.MainValue), RxApp.MainThreadScheduler).Subscribe(x =>
            {
                ToleranceMax = x;
                if (ToleranceValue > x) ToleranceValue = x;
            });
        }

        public void LoadData(int min, int max)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                MainValue = (max + min) / 2;
                ToleranceValue = (max - min) / 2;
            });
        }

        public (int, int) GetData()
        {
            var min = MainValue - ToleranceValue;
            var max = MainValue + ToleranceValue;
            return (min, max);
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