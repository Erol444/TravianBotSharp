using ReactiveUI;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.Store
{
    public class VillageTabStore : ViewModelBase
    {
        private readonly bool[] _tabVisibility = new bool[4];
        private TabType _currentTabType;

        public VillageTabStore()
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

            IsNoVillageTabVisible = _tabVisibility[(int)TabType.NoAccount];
            IsNormalTabVisible = _tabVisibility[(int)TabType.Normal];

            switch (tabType)
            {
                case TabType.NoAccount:
                    IsNoVillageTabSelected = true;
                    break;

                case TabType.Normal:
                    IsNormalTabSelected = true;
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

        private bool _isNoVillageTabSelected;

        public bool IsNoVillageTabSelected
        {
            get => _isNoVillageTabSelected;
            set => this.RaiseAndSetIfChanged(ref _isNoVillageTabSelected, value);
        }

        private bool _isNormalTabSelected;

        public bool IsNormalTabSelected
        {
            get => _isNormalTabSelected;
            set => this.RaiseAndSetIfChanged(ref _isNormalTabSelected, value);
        }
    }
}