using ReactiveUI;
using System;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Store
{
    public class NavigationStore : ReactiveObject
    {
        public NavigationStore()
        {
            CurrentViewModel = new NoAccountViewModel();
        }

        public void Change(Func<ViewModelBase> createViewModel)
        {
            CurrentViewModel = createViewModel();
        }

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }
    }
}