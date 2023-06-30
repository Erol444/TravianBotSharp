using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc.MainView;

namespace WPFUI.Store
{
    public abstract class NavigationStore : ReactiveObject
    {
        public event Action<TabHeaderViewModel[]> OnTabChanged;

        private TabType _currentTab;

        protected Dictionary<TabType, TabHeaderViewModel[]> _tabsHolder;
        private List<ActivatableViewModelBase> _viewModelList;

        public void Init()
        {
            InitTabHolder();
            InitViewModelList();
        }

        protected abstract void InitTabHolder();

        private void InitViewModelList()
        {
            _viewModelList = new();
            foreach (var tabs in _tabsHolder.Values)
            {
                foreach (var tab in tabs)
                {
                    _viewModelList.Add(tab.ViewModel);
                }
            }
        }

        public void SetTab(TabType tab)
        {
            if (_currentTab == tab) return;

            _currentTab = tab;
            TabHeaders = _tabsHolder[tab];
            OnTabChanged?.Invoke(_tabsHolder[tab]);
        }

        public void ClearSelect()
        {
            if (_tabHeaders is null) return;
            foreach (var tab in _tabHeaders)
            {
                tab.IsSelected = false;
            }
        }

        private TabHeaderViewModel[] _tabHeaders;

        public TabHeaderViewModel[] TabHeaders
        {
            get => _tabHeaders;
            set
            {
                ClearSelect();
                _tabHeaders = value;
                _tabHeaders.First().Select(true);
            }
        }

        public List<ActivatableViewModelBase> ViewModelList => _viewModelList;
    }
}