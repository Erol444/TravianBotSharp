using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class ActivatableViewModelBase : ViewModelBase
    {
        public ActivatableViewModelBase()
        {
            this.WhenAnyValue(vm => vm.IsActive)
                .Where(isActive => isActive == true)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(_ => OnActive());

            this.WhenAnyValue(vm => vm.IsActive)
                .Where(isActive => isActive == false)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(_ => OnDeactive());
        }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        protected virtual void OnActive()
        {
        }

        protected virtual void OnDeactive()
        {
        }
    }
}