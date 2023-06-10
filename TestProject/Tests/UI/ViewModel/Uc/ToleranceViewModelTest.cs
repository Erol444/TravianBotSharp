using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.Tests.Mock.ViewModel.Uc;
using WPFUI.ViewModels.Uc;

namespace TestProject.Tests.UI.ViewModel.Uc
{
    [TestClass]
    public class ToleranceViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            var vm = new ToleranceViewModel();
            var min = FakeMinMaxData.Min;
            var max = FakeMinMaxData.Max;
            vm.LoadData(min, max);

            var data = vm.GetData();
            Assert.AreEqual(min, data.Item1);
            Assert.AreEqual(max, data.Item2);
        }
    }
}