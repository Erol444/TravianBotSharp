using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.Mock.ViewModel.Uc;
using WPFUI.ViewModels.Uc;

namespace TestProject.Tests.UI.ViewModel.Uc
{
    [TestClass]
    public class ResourcesViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            var vm = new ResourcesViewModel();
            var wood = FakeResourceData.Wood;
            var clay = FakeResourceData.Clay;
            var iron = FakeResourceData.Iron;
            var crop = FakeResourceData.Crop;
            vm.LoadData(wood, clay, iron, crop);

            var data = vm.GetData();
            Assert.AreEqual(wood, data.Item1);
            Assert.AreEqual(clay, data.Item2);
            Assert.AreEqual(iron, data.Item3);
            Assert.AreEqual(crop, data.Item4);
        }
    }
}