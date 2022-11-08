using AvaloniaEdit.Utils;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using UI.ViewModels.Tabs;
using UI.Views.Tabs;

namespace UI.ViewModels.UserControls
{
    public sealed class TabPanelViewModel : ViewModelBase
    {
        public TabPanelViewModel(AccountViewModel accountViewModel)
        {
            accountViewModel.AccountChanged += OnAccountChanged;
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
                    TabType.AddAccounts, new TabItemViewModel[]
                    {
                        new("Add accounts", Locator.Current.GetService<AddAccountsTab>()),
                    }
                },
                {
                    TabType.EditAccount, new TabItemViewModel[]
                    {
                        new("Edit account", Locator.Current.GetService<EditAccountTab>()),
                    }
                },
                {
                    TabType.Normal, new TabItemViewModel[]
                    {
                        new("Settings", Locator.Current.GetService<SettingsTab>()),
                        new("Debug", Locator.Current.GetService<DebugTab>()),
                    }
                }
            };

            Tabs = new()
            {
                _tabsHolder[TabType.NoAccount][0],
            };
        }

        private void OnAccountChanged(int obj)
        {
            if (_current == TabType.EditAccount) return;
            SetTab(TabType.Normal);
        }

        public void SetTab(TabType type)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Tabs.Clear();
                Tabs.AddRange(_tabsHolder[type]);
                _current = type;
            });
        }

        private readonly Dictionary<TabType, TabItemViewModel[]> _tabsHolder;
        private TabType _current;

        public ObservableCollection<TabItemViewModel> Tabs { get; }
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