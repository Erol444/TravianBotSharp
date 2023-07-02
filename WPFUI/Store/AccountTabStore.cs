using ReactiveUI;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.Store
{
    public class AccountTabStore : ViewModelBase
    {
        private readonly bool[] _tabVisibility = new bool[4];
        private TabType _currentTabType;

        public AccountTabStore()
        {
            SetTabType(TabType.NoAccount);
        }

        public void SetTabType(TabType tabType)
        {
            if (tabType == _currentTabType) return;
            _currentTabType = tabType;

            for (int i = 0; i < _tabVisibility.Length; i++)
            {
                _tabVisibility[i] = false;
            }
            _tabVisibility[(int)tabType] = true;

            IsNoAccountTabVisible = _tabVisibility[(int)TabType.NoAccount];
            IsAddAccountTabVisible = _tabVisibility[(int)TabType.AddAccount];
            IsAddAccountsTabVisible = _tabVisibility[(int)TabType.AddAccounts];
            IsNormalTabVisible = _tabVisibility[(int)TabType.Normal];

            switch (tabType)
            {
                case TabType.NoAccount:
                    IsNoAccountTabSelected = true;
                    break;

                case TabType.Normal:
                    IsNormalTabSelected = true;
                    break;

                case TabType.AddAccount:
                    IsAddAccountTabSelected = true;
                    break;

                case TabType.AddAccounts:
                    IsAddAccountsTabSelected = true;
                    break;

                default:
                    break;
            }
        }

        private bool _isNoAccountTabVisible;

        public bool IsNoAccountTabVisible
        {
            get => _isNoAccountTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNoAccountTabVisible, value);
        }

        private bool _isAddAccountTabVisible;

        public bool IsAddAccountTabVisible
        {
            get => _isAddAccountTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isAddAccountTabVisible, value);
        }

        private bool _isAddAccountsTabVisible;

        public bool IsAddAccountsTabVisible
        {
            get => _isAddAccountsTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isAddAccountsTabVisible, value);
        }

        private bool _isNormalTabVisible;

        public bool IsNormalTabVisible
        {
            get => _isNormalTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNormalTabVisible, value);
        }

        private bool _isNoAccountTabSelected;

        public bool IsNoAccountTabSelected
        {
            get => _isNoAccountTabSelected;
            set => this.RaiseAndSetIfChanged(ref _isNoAccountTabSelected, value);
        }

        private bool _isAddAccountTabSelected;

        public bool IsAddAccountTabSelected
        {
            get => _isAddAccountTabSelected;
            set => this.RaiseAndSetIfChanged(ref _isAddAccountTabSelected, value);
        }

        private bool _isAddAccountsTabSelected;

        public bool IsAddAccountsTabSelected
        {
            get => _isAddAccountsTabSelected;
            set => this.RaiseAndSetIfChanged(ref _isAddAccountsTabSelected, value);
        }

        private bool _isNormalTabSelected;

        public bool IsNormalTabSelected
        {
            get => _isNormalTabSelected;
            set => this.RaiseAndSetIfChanged(ref _isNormalTabSelected, value);
        }
    }
}