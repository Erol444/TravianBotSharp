using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class AccountTabBaseViewModel : TabBaseViewModel
    {
        public AccountTabBaseViewModel() : base()
        {
            this.WhenAnyValue(x => x.AccountId).Subscribe(LoadData);
        }

        protected abstract void LoadData(int index);

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }
    }
}