using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCoreTest.Factories
{
    internal class BuildingFactory
    {
        public Village CreateVillage()
        {
            return new Village()
            {
                Build = new VillBuilding()
                {
                    Buildings = CreateBuildingList().ToArray(),
                    Tasks = CreateBuildingTasks(),
                },
                Res = new VillRes()
                {
                    Production = new TbsCore.Models.ResourceModels.Resources(100, 110, 100, 50)
                }
            };
        }

        public List<BuildingTask> CreateBuildingTasks()
        {
            return new List<BuildingTask>
            {
                new BuildingTask() { Building = BuildingEnum.Barracks, Level = 5 }
            };
        }

        public List<Building> CreateResBuildingList()
        {
            return new List<Building>
            {
                new Building() { Id = 1, Type = BuildingEnum.Woodcutter, Level = 2, UnderConstruction = false },
                new Building() { Id = 3, Type = BuildingEnum.ClayPit, Level = 2, UnderConstruction = false },
                new Building() { Id = 6, Type = BuildingEnum.ClayPit, Level = 2, UnderConstruction = true },
                new Building() { Id = 2, Type = BuildingEnum.IronMine, Level = 2, UnderConstruction = false },
                new Building() { Id = 9, Type = BuildingEnum.Cropland, Level = 2, UnderConstruction = true },
                new Building() { Id = 0, Type = BuildingEnum.Cropland, Level = 2, UnderConstruction = false },
                new Building() { Id = 4, Type = BuildingEnum.Cropland, Level = 3, UnderConstruction = true },
                new Building() { Id = 5, Type = BuildingEnum.Cropland, Level = 3, UnderConstruction = false }
            };
        }

        public List<Building> CreateBuildingList()
        {
            var list = CreateResBuildingList();
            list.Add(new Building() { Id = 20, Type = BuildingEnum.MainBuilding, Level = 5, UnderConstruction = false });
            list.Add(new Building() { Id = 21, Type = BuildingEnum.Academy, Level = 1, UnderConstruction = false });
            list.Add(new Building() { Id = 22, Type = BuildingEnum.Barracks, Level = 3, UnderConstruction = false });
            list.Add(new Building() { Id = 23, Type = BuildingEnum.RallyPoint, Level = 1, UnderConstruction = false });
            list.Add(new Building() { Id = 24, Type = BuildingEnum.Warehouse, Level = 4, UnderConstruction = false });
            list.Add(new Building() { Id = 25, Type = BuildingEnum.Granary, Level = 3, UnderConstruction = false });
            return list;
        }
    }
}
