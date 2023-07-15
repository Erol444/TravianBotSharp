using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Abstract
{
    public class TabBaseViewModel : ViewModelBase
    {
        private bool _isActive;

        public TabBaseViewModel()
        {
            this.WhenAnyValue(x => x.IsActive)
                .Subscribe(isActive =>
                {
                    if (isActive)
                    {
                        OnActive();
                    }
                    else
                    {
                        OnDeactive();
                    }
                });
        }

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