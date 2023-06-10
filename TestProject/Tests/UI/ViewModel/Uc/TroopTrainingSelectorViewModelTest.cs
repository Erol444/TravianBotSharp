using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.Tests.Mock.ViewModel.Uc;
using WPFUI.ViewModels.Uc;

namespace TestProject.Tests.UI.ViewModel.Uc
{
    [TestClass]
    public class TroopTrainingSelectorViewModelTest
    {
        [TestMethod]
        public void InitTest()
        {
            _ = new TroopTrainingSelectorViewModel();
        }

        [TestMethod]
        public void LoadDataTest()
        {
            var vm = new TroopTrainingSelectorViewModel();
            var troops = FakeTroopTrainingSelectorData.GetTroopInfos();
            var selectedTroop = FakeTroopTrainingSelectorData.GetSelectedTroopInfo();
            var min = FakeTroopTrainingSelectorData.GetMin();
            var max = FakeTroopTrainingSelectorData.GetMax();
            var isGreat = FakeTroopTrainingSelectorData.GetIsGreat();
            vm.LoadData(troops, selectedTroop, min, max, isGreat);

            var data = vm.GetData();

            Assert.AreEqual(selectedTroop, data.Item1);
            Assert.AreEqual(min, data.Item2);
            Assert.AreEqual(max, data.Item3);
            Assert.AreEqual(isGreat, data.Item4);
        }
    }
}