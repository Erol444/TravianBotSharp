using System;
using TbsCore.Helpers;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCoreTest.Factories;
using TbsCore.Tasks.LowLevel;
using Xunit;
using System.Linq;
using static TbsCore.Helpers.Classificator;

namespace TbsCoreTest
{
    public class BuildingTest
    {
        [Fact]
        internal void Test()
        {
            var factory = new BuildingFactory();
            var vill = factory.CreateVillage();

            Assert.True(BuildingHelper.BuildingAboveLevel(BuildingEnum.Academy, 1, vill), "Academy should be lvl 1");
            Assert.False(BuildingHelper.BuildingAboveLevel(BuildingEnum.Academy, 2, vill), "Academy should be lvl 1");
            Assert.True(BuildingHelper.BuildingAboveLevel(BuildingEnum.Barracks, 2, vill), "Barracks is lvl 3");
            Assert.True(BuildingHelper.BuildingAboveLevel(BuildingEnum.Barracks, 3, vill), "Barracks is lvl 3");
            Assert.True(BuildingHelper.BuildingAboveLevel(BuildingEnum.Barracks, 5, vill), "Barracks building task to level 5");
            Assert.False(BuildingHelper.BuildingAboveLevel(BuildingEnum.Barracks, 6, vill), "Barracks building task to level 5");

            var croplands = factory.CreateBuildingList().Where(x => x.Type == BuildingEnum.Cropland).ToList();
            Assert.Equal(0, BuildingHelper.FindLowestLevelBuilding(croplands).Id);
            croplands.Add(new Building() { Id = 15, Level = 1 });
            Assert.Equal(15, BuildingHelper.FindLowestLevelBuilding(croplands).Id);
            croplands.Add(new Building() { Id = 15, Level = 1, UnderConstruction = true });
            Assert.Equal(15, BuildingHelper.FindLowestLevelBuilding(croplands).Id);

            var resList = factory.CreateResBuildingList();
            Assert.Equal(0, BuildingHelper.GetLowestProduction(resList, vill).Id);
            vill.Res.Production.Clay = 40;
            Assert.Equal(3, BuildingHelper.GetLowestProduction(resList, vill).Id);
            //BuildingHelper.GetLowestRes()
            // BuildingHelper.BuildingRequirementsAreMet() // test this
            //BuildingHelper.IsTaskCompleted(vill,)
            //BuildingHelper.CheckExcludeCrop();
        }
    }
}