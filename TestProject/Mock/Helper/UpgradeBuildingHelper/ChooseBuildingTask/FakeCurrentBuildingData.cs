using MainCore.Enums;
using MainCore.Models.Database;
using System.Collections.Generic;

namespace TestProject.Mock.Helper.UpgradeBuildingHelper.ChooseBuildingTask
{
    public class FakeCurrentBuildingData
    {
        public static List<VillCurrentBuilding> Empty => new();

        public static List<VillCurrentBuilding> One => new()
        {
            new VillCurrentBuilding()
            {
                Id = 1,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.Cropland,
                Level = 2,
            },
        };

        public static List<VillCurrentBuilding> Two => new()
        {
            new VillCurrentBuilding()
            {
                Id = 1,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.Cropland,
                Level = 2,
            },
            new VillCurrentBuilding()
            {
                Id = 2,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.MainBuilding,
                Level = 2,
            },
        };

        public static List<VillCurrentBuilding> TwoCropOnly => new()
        {
            new VillCurrentBuilding()
            {
                Id = 1,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.Cropland,
                Level = 2,
            },
            new VillCurrentBuilding()
            {
                Id = 2,
                VillageId = FakeIdData.VillageId,
                Location = 3,
                Type = BuildingEnums.Cranny,
                Level = 2,
            },
        };

        public static List<VillCurrentBuilding> TwoBuildingOnly => new()
        {
            new VillCurrentBuilding()
            {
                Id = 1,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.MainBuilding,
                Level = 2,
            },
            new VillCurrentBuilding()
            {
                Id = 2,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.MainBuilding,
                Level = 4,
            },
        };

        public static List<VillCurrentBuilding> Full => new()
        {
            new VillCurrentBuilding()
            {
                Id = 1,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.Cropland,
                Level = 2,
            },
            new VillCurrentBuilding()
            {
                Id = 2,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.MainBuilding,
                Level = 2,
            },
             new VillCurrentBuilding()
            {
                Id =3,
                VillageId = FakeIdData.VillageId,
                Location = 1,
                Type = BuildingEnums.MainBuilding,
                Level = 3,
            },
        };
    }
}