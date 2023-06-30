using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class TabHeaderViewModel : ViewModelBase
    {
        private string _content;
        private bool _isSelected;
        private readonly ObservableAsPropertyHelper<bool> _isNotSelected;

        public ReactiveCommand<Unit, Unit> ClickCommand { get; }

        private readonly ActivatableViewModelBase _viewModel;
        private readonly NavigationStore _navigationStore;

        public TabHeaderViewModel(string content, ActivatableViewModelBase viewModel, NavigationStore navigationStore)
        {
            _viewModel = viewModel;
            Content = content;
            _navigationStore = navigationStore;
            ClickCommand = ReactiveCommand.Create(() => Select());

            this.WhenAnyValue(vm => vm.IsSelected)
                .Select(x => !x)
                .ToProperty(this, vm => vm.IsNotSelected, out _isNotSelected);

            this.WhenAnyValue(vm => vm.IsSelected)
                .BindTo(_viewModel, vm => vm.IsActive);
        }

        public ActivatableViewModelBase ViewModel => _viewModel;

        public void Select(bool isForce = false)
        {
            if (IsSelected && !isForce) return;
            _navigationStore.ClearSelect();
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

        public bool IsNotSelected
        {
            get => _isNotSelected.Value;
        }
    }
}