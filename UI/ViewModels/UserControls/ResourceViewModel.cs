using ReactiveUI;
using UI.Models;

namespace UI.ViewModels.UserControls
{
    public class ResourceViewModel : ReactiveObject
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, $"{value}: ");
        }

        private ResourcesModel _resources;

        public ResourcesModel Resources
        {
            get => _resources;
            set => this.RaiseAndSetIfChanged(ref _resources, value);
        }
    }
}