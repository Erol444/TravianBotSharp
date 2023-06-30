using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.MainView
{
    public class MainTabPanelViewModel : ActivatableViewModelBase
    {
        public List<ActivatableViewModelBase> TabContents { get; }
        public ObservableCollection<TabHeaderViewModel> TabHeaders { get; } = new();

        public MainTabPanelViewModel(AccountNavigationStore navigationStore)
        {
            navigationStore.OnTabChanged += OnTabChanged;
            TabContents = navigationStore.ViewModelList;
        }

        private void OnTabChanged(TabHeaderViewModel[] tabHeaders)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                TabHeaders.Clear();
                TabHeaders.AddRange(tabHeaders);
            });
        }
    }
}