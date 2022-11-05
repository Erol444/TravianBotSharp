using ReactiveUI;
using UI.Models;

namespace UI.ViewModels.UserControls
{
    public class ResourceWithStorageViewModel : ReactiveObject
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, $"{value}: ");
        }

        private ResourcesWithStorageModel _resources;

        public ResourcesWithStorageModel Resources
        {
            get => _resources;
            set => this.RaiseAndSetIfChanged(ref _resources, value);
        }
    }
}