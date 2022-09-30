using MainCore.Models.Database;
using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class AccountTabBaseViewModel : TabBaseViewModel
    {
        public AccountTabBaseViewModel() : base()
        {
            this.WhenAnyValue(x => x.CurrentAccount).Subscribe(x =>
            {
                if (x is not null)
                {
                    LoadData(x.Id);
                }
            });
        }

        protected abstract void LoadData(int index);

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }
    }
}