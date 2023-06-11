using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFUI.ViewModels;

namespace TestProject.Tests.UI.ViewModel
{
    [TestClass]
    public class SelectorViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            var vm = new SelectorViewModel();
            _ = vm;
        }
    }
}