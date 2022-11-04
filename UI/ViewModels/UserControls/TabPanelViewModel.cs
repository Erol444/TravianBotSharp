using AvaloniaEdit.Utils;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using UI.ViewModels.Tabs;
using UI.Views.Tabs;

namespace UI.ViewModels.UserControls
{
    public sealed class TabPanelViewModel : ViewModelBase
    {
        public TabPanelViewModel(AccountViewModel accountViewModel)
        {
            _accountViewModel = accountViewModel;

            _tabsHolder = new Dictionary<TabType, TabItemViewModel[]>()
            {
                {
                    TabType.NoAccount, new TabItemViewModel[]
                    {
                        new("No account", Locator.Current.GetService<NoAccountTab>()),
                    }
                },
                {
                    TabType.AddAccount, new TabItemViewModel[]
                    {
                        new("Add account", Locator.Current.GetService<AddAccountTab>()),
                    }
                },
                {
                    TabType.EditAccount, new TabItemViewModel[]
                    {
                        new("Edit account", Locator.Current.GetService<EditAccountTab>()),
                    }
                }
            };

            Tabs = new()
            {
                _tabsHolder[TabType.NoAccount][0],
            };

            this.WhenAnyValue(vm => vm._accountViewModel.IsAccountNotSelected).Subscribe(x =>
            {
                if (x) SetTab(TabType.AddAccount);
            });
        }

        public void SetTab(TabType type)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tabs.Clear();
                Tabs.AddRange(_tabsHolder[type]);
            });
        }

        private readonly Dictionary<TabType, TabItemViewModel[]> _tabsHolder;

        public ObservableCollection<TabItemViewModel> Tabs { get; }
        private readonly AccountViewModel _accountViewModel;
    }

    public enum TabType
    {
        NoAccount,
        Normal,
        AddAccount,
        AddAccounts,
        EditAccount
    }
}