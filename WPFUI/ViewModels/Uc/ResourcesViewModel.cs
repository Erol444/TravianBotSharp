using ReactiveUI;
using System.Reactive.Concurrency;

namespace WPFUI.ViewModels.Uc
{
    public class ResourcesViewModel : ReactiveObject
    {
        public void LoadData(int wood, int clay, int iron, int crop)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Wood = wood;
                Clay = clay;
                Iron = iron;
                Crop = crop;
            });
        }

        public (int, int, int, int) GetData()
        {
            return (Wood, Clay, Iron, Crop);
        }

        private int _wood;

        public int Wood
        {
            get => _wood;
            set => this.RaiseAndSetIfChanged(ref _wood, value);
        }

        private int _clay;

        public int Clay
        {
            get => _clay;
            set => this.RaiseAndSetIfChanged(ref _clay, value);
        }

        private int _iron;

        public int Iron
        {
            get => _iron;
            set => this.RaiseAndSetIfChanged(ref _iron, value);
        }

        private int _crop;

        public int Crop
        {
            get => _crop;
            set => this.RaiseAndSetIfChanged(ref _crop, value);
        }
    }
}