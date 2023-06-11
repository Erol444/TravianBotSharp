using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.Tests.Mock.ViewModel.Uc;
using WPFUI.ViewModels.Uc;

namespace TestProject.Tests.UI.ViewModel.Uc
{
    [TestClass]
    public class CheckBoxWithInputViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            var vm = new CheckBoxWithInputViewModel();
            var isChecked = FakeCheckboxData.IsChecked;
            var value = FakeCheckboxData.Value;
            vm.LoadData(isChecked, value);
            var data = vm.GetData();
            Assert.AreEqual(isChecked, data.Item1);
            Assert.AreEqual(value, data.Item2);
        }
    }
}