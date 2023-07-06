using ReactiveUI;
using System.Reactive.Concurrency;

namespace WPFUI.ViewModels.Uc
{
    public class ResourcesViewModel : ReactiveObject
    {
        public void LoadData(long wood, long clay, long iron, long crop)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Wood = wood;
                Clay = clay;
                Iron = iron;
                Crop = crop;
            });
        }

        public (long, long, long, long) GetData()
        {
            return (Wood, Clay, Iron, Crop);
        }

        private long _wood;

        public long Wood
        {
            get => _wood;
            set => this.RaiseAndSetIfChanged(ref _wood, value);
        }

        private long _clay;

        public long Clay
        {
            get => _clay;
            set => this.RaiseAndSetIfChanged(ref _clay, value);
        }

        private long _iron;

        public long Iron
        {
            get => _iron;
            set => this.RaiseAndSetIfChanged(ref _iron, value);
        }

        private long _crop;

        public long Crop
        {
            get => _crop;
            set => this.RaiseAndSetIfChanged(ref _crop, value);
        }
    }
}