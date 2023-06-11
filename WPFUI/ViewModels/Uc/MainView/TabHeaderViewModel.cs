using ReactiveUI;
using System.Reactive;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class TabHeaderViewModel : ViewModelBase
    {
        private string _content;
        private bool _isSelected;
        public ReactiveCommand<Unit, Unit> ClickCommand { get; }

        private readonly NavigationStore _navigationStore;
        private readonly ViewModelBase _viewModel;

        public TabHeaderViewModel(string content, ViewModelBase viewModel, NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _viewModel = viewModel;
            Content = content;

            ClickCommand = ReactiveCommand.Create(() => Select());
        }

        public void Select(bool isForce = false)
        {
            if (IsSelected && !isForce) return;

            _navigationStore.Change(_viewModel);
            IsSelected = true;
        }

        public string Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
    }
}