using ReactiveUI;

namespace UI.Models
{
    public class ResourcesWithStorageModel : ResourcesModel
    {
        private int _warehouse;
        private int _granary;

        public int Warehouse
        {
            get => _warehouse;
            set => this.RaiseAndSetIfChanged(ref _warehouse, value);
        }

        public int Granary
        {
            get => _granary;
            set => this.RaiseAndSetIfChanged(ref _granary, value);
        }
    }
}