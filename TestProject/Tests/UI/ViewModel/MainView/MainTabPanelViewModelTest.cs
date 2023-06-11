using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI.Testing;
using WPFUI.ViewModels;
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
                var selectorVm = new SelectorViewModel();
                var vm = new MainTabPanelViewModel(selectorVm);
                _ = vm;
            });
        }
    }
}