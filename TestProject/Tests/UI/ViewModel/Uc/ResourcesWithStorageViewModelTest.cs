using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.Mock.ViewModel.Uc;
using WPFUI.ViewModels.Uc;

namespace TestProject.Tests.UI.ViewModel.Uc
{
    [TestClass]
    public class ResourcesWithStorageViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            var vm = new ResourcesWithStorageViewModel();
            var warehouse = FakeResourceData.Warehouse;
            var granary = FakeResourceData.Granary;
            var wood = FakeResourceData.Wood;
            var clay = FakeResourceData.Clay;
            var iron = FakeResourceData.Iron;
            var crop = FakeResourceData.Crop;
            vm.LoadData(warehouse, granary, wood, clay, iron, crop);

            var data = vm.GetData();
            Assert.AreEqual(warehouse, data.Item1);
            Assert.AreEqual(granary, data.Item2);
            Assert.AreEqual(wood, data.Item3);
            Assert.AreEqual(clay, data.Item4);
            Assert.AreEqual(iron, data.Item5);
            Assert.AreEqual(crop, data.Item6);
        }
    }
}