using ReactiveUI;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Store
{
    public class NavigationStore : ReactiveObject
    {
        public NavigationStore()
        {
            CurrentViewModel = new NoAccountViewModel();
        }

        public void Change(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
            foreach (var tab in TabHeaders)
            {
                tab.IsSelected = false;
            }
        }

        public TabHeaderViewModel[] TabHeaders { get; set; }

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }
    }
}