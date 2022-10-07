using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class ResourcesViewModel : ReactiveObject
    {
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

        private readonly string _crop;

        public string Crop
        {
            get => _iron;
            set => this.RaiseAndSetIfChanged(ref _iron, value);
        }
    }
}