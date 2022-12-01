using ReactiveUI;
using System.Windows.Controls;

namespace WPFUI.ViewModels.Tabs
{
    public sealed class TabItemViewModel : ReactiveObject
    {
        public TabItemViewModel(string header, Page content)
        {
            Header = header;
            Content = content;
        }

        public string Header { get; set; }
        public Page Content { get; set; }
        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
    }
}