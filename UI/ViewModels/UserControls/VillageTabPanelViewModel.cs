using AvaloniaEdit.Utils;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using UI.ViewModels.Tabs;
using UI.Views.Tabs.Villages;

namespace UI.ViewModels.UserControls
{
    public class VillageTabPanelViewModel : ViewModelBase
    {
        public VillageTabPanelViewModel(VillageViewModel villageViewModel)
        {
            _tabsHolder = new Dictionary<TabType, TabItemViewModel[]>()
            {
                {
                    TabType.NoAccount, new TabItemViewModel[]
                    {
                        new("No village", Locator.Current.GetService<NoVillageTab>()),
                    }
                },
                {
                    TabType.Normal, new TabItemViewModel[]
                    {
                        new("Build", Locator.Current.GetService<BuildTab>()),
                    }
                }
            };

            villageViewModel.VillageChanged += OnVillageChanged;
        }

        private void OnVillageChanged(int obj)
        {
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

        public ObservableCollection<TabItemViewModel> Tabs { get; } = new();
    }
}