using ReactiveUI;

namespace WPFUI.ViewModels.Uc
{
    public class ResourcesViewModel : ReactiveObject
    {
        public ResourcesViewModel(string text) : base()
        {
            Text = text;
        }

        private string _text;

        public string Text
        {
            get => $"{_text}: ";
            set => this.RaiseAndSetIfChanged(ref _text, value);
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