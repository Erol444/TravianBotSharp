using Avalonia.Controls;
using ReactiveUI;

namespace UI.ViewModels.Tabs
{
    public sealed class TabItemViewModel : ReactiveObject
    {
        public TabItemViewModel(string header, UserControl content)
        {
            Header = header;
            Content = content;
        }

        public string Header { get; set; }
        public UserControl Content { get; set; }
        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }
    }
}