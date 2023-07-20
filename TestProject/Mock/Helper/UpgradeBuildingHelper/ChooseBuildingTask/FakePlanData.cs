using MainCore.Enums;
using MainCore.Models.Runtime;
using System.Collections.Generic;

namespace TestProject.Mock.Helper.UpgradeBuildingHelper.ChooseBuildingTask
{
    public class FakePlanData
    {
        public static List<PlanTask> ResourceFirst => new()
        {
            new PlanTask()
            {
                Location = 10,
                Building = BuildingEnums.Cropland,
                Level = 2,
            },
            new PlanTask()
            {
                Location = 1,
                Building = BuildingEnums.MainBuilding,
                Level = 2,
            },
        };

        public static List<PlanTask> BuildingFirst => new()
        {
            new PlanTask()
            {
                Location = 1,
                Building = BuildingEnums.MainBuilding,
                Level = 2,
            },
            new PlanTask()
            {
                Location = 10,
                Building = BuildingEnums.Cropland,
                Level = 2,
            },
        };

        public static List<PlanTask> ResourceFirstButAutoResource => new()
        {
            new PlanTask()
            {
                BuildingStrategy = BuildingStrategyEnums.BasedOnLevel,
                ResourceType = ResTypeEnums.AllResources,
                Level = 2,
            },
            new PlanTask()
            {
                Location = 1,
                Building = BuildingEnums.MainBuilding,
                Level = 2,
            },
        };

        public static List<PlanTask> BuildingFirstButAutoResource => new()
        {
            new PlanTask()
            {
                Location = 1,
                Building = BuildingEnums.MainBuilding,
                Level = 2,
            },
            new PlanTask()
            {
                BuildingStrategy = BuildingStrategyEnums.BasedOnLevel,
                ResourceType = ResTypeEnums.AllResources,
                Level = 2,
            },
        };
    }
}