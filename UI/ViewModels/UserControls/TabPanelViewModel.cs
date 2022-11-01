using Splat;
using UI.ViewModels.Tabs;
using UI.Views.Tabs;

namespace UI.ViewModels.UserControls
{
    public sealed class TabPanelViewModel : ViewModelBase
    {
        public TabPanelViewModel()
        {
            Tabs = new TabItemViewModel[]
            {
                new ("No account", Locator.Current.GetService<NoAccountTab>()),
                new ("Add account", Locator.Current.GetService<AddAccountTab>()),
            };
        }

        public TabItemViewModel[] Tabs { get; private set; }
    }
}