using ReactiveUI;
using System.Reactive.Linq;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class ResourcesWithStorageViewModel : ViewModelBase
    {
        public void LoadData(long warehouse, long granary, long wood, long clay, long iron, long crop)
        {
            Observable.Start(() =>
            {
                Warehouse = warehouse;
                Granary = granary;
                Wood = wood;
                Clay = clay;
                Iron = iron;
                Crop = crop;
            }, RxApp.MainThreadScheduler);
        }

        public (long, long, long, long, long, long) GetData()
        {
            return (Warehouse, Granary, Wood, Clay, Iron, Crop);
        }

        private long _warehouse;

        public long Warehouse
        {
            get => _warehouse;
            set => this.RaiseAndSetIfChanged(ref _warehouse, value);
        }

        private long _granary;

        public long Granary
        {
            get => _granary;
            set => this.RaiseAndSetIfChanged(ref _granary, value);
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