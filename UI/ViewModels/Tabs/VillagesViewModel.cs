using ReactiveUI;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.ViewModels.Tabs
{
    public class VillagesViewModel : ActivatableViewModelBase
    {
        public VillagesViewModel(VillagesTableViewModel villagesTableViewModel, VillageTabPanelViewModel villageTabPanelViewModel)
        {
            _villagesTableViewModel = villagesTableViewModel;
            _villageTabPanelViewModel = villageTabPanelViewModel;
        }

        protected override void OnActived(CompositeDisposable disposable)
        {
            base.OnActived(disposable);
            _villageTabPanelViewModel.SetTab(TabType.NoAccount);
            RxApp.MainThreadScheduler.Schedule(async () => await _villagesTableViewModel.LoadTask());
        }

        private readonly VillagesTableViewModel _villagesTableViewModel;
        private readonly VillageTabPanelViewModel _villageTabPanelViewModel;
    }
}