using ReactiveUI;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Store
{
    public class AccountTabStore : ViewModelBase
    {
        private readonly bool[] _tabVisibility = new bool[4];
        private TabType _currentTabType;

        private readonly NoAccountViewModel _noAccountViewModel;
        private readonly AddAccountViewModel _addAccountViewModel;
        private readonly AddAccountsViewModel _addAccountsViewModel;
        private readonly SettingsViewModel _normalAccountViewModel;

        public AccountTabStore(NoAccountViewModel noAccountViewModel, AddAccountViewModel addAccountViewModel, AddAccountsViewModel addAccountsViewModel, SettingsViewModel normalAccountViewModel)
        {
            _noAccountViewModel = noAccountViewModel;
            _addAccountViewModel = addAccountViewModel;
            _addAccountsViewModel = addAccountsViewModel;
            _normalAccountViewModel = normalAccountViewModel;

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
                    _noAccountViewModel.IsActive = true;
                    break;

                case TabType.Normal:
                    _normalAccountViewModel.IsActive = true;
                    break;

                case TabType.AddAccount:
                    _addAccountViewModel.IsActive = true;
                    break;

                case TabType.AddAccounts:
                    _addAccountsViewModel.IsActive = true;
                    break;

                default:
                    break;
            }
        }

        private bool _isNoAccountTabVisible = true;

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
    }
}