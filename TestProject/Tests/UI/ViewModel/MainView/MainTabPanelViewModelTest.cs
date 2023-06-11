using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI.Testing;
using WPFUI.Store;
using WPFUI.ViewModels.Uc.MainView;

namespace TestProject.Tests.UI.ViewModel.MainView
{
    [TestClass]
    public class MainTabPanelViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            new TestScheduler().With(scheduler =>
            {
                var navigationStore = new NavigationStore();
                var vm = new MainTabPanelViewModel(navigationStore);
                _ = vm;
            });
        }
    }
}