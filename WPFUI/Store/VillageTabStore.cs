using ReactiveUI;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Store
{
    public class VillageTabStore : ViewModelBase
    {
        private readonly bool[] _tabVisibility = new bool[4];
        private TabType _currentTabType;

        private readonly NoVillageViewModel _noVillageViewModel;
        private readonly BuildViewModel _normalVillageViewModel;

        public VillageTabStore(NoVillageViewModel noVillageViewModel, BuildViewModel normalVillageViewModel)
        {
            _noVillageViewModel = noVillageViewModel;
            _normalVillageViewModel = normalVillageViewModel;

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

            IsNoVillageTabVisible = _tabVisibility[(int)TabType.NoAccount];
            IsNormalTabVisible = _tabVisibility[(int)TabType.Normal];

            switch (tabType)
            {
                case TabType.NoAccount:
                    _noVillageViewModel.IsActive = true;
                    break;

                case TabType.Normal:
                    _normalVillageViewModel.IsActive = true;
                    break;

                default:
                    break;
            }
        }

        private bool _isNoVillageTabVisible;

        public bool IsNoVillageTabVisible
        {
            get => _isNoVillageTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNoVillageTabVisible, value);
        }

        private bool _isNormalTabVisible;

        public bool IsNormalTabVisible
        {
            get => _isNormalTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNormalTabVisible, value);
        }
    }
}