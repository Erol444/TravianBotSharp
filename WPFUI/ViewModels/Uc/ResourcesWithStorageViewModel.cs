using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class ResourcesWithStorageViewModel : ReactiveObject
    {
        private string _warehouse;

        public string Warehouse
        {
            get => _warehouse;
            set => this.RaiseAndSetIfChanged(ref _warehouse, value);
        }

        private string _granary;

        public string Granary
        {
            get => _granary;
            set => this.RaiseAndSetIfChanged(ref _granary, value);
        }

        private string _wood;

        public string Wood
        {
            get => _wood;
            set => this.RaiseAndSetIfChanged(ref _wood, value);
        }

        private string _clay;

        public string Clay
        {
            get => _clay;
            set => this.RaiseAndSetIfChanged(ref _clay, value);
        }

        private string _iron;

        public string Iron
        {
            get => _iron;
            set => this.RaiseAndSetIfChanged(ref _iron, value);
        }

        private string _crop;

        public string Crop
        {
            get => _crop;
            set => this.RaiseAndSetIfChanged(ref _crop, value);
        }
    }
}