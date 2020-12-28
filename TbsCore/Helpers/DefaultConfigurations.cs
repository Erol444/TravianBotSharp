using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Tasks;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Helpers
{
    public static class DefaultConfigurations
    {
        public static void DeffVillagePlan(Account acc, Village vill)
        {
            FarmVillagePlan(acc, vill);
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Barracks, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Academy, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Smithy, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Marketplace, Level = 15 });
        }
        public static void FarmVillagePlan(Account acc, Village vill)
        {
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.AutoUpgradeResFields, Level = 10, ResourceType = ResTypeEnum.AllResources, BuildingStrategy = BuildingStrategyEnum.BasedOnRes });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.RallyPoint, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.MainBuilding, Level = 3 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 3 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 3 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Marketplace, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 4 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 4 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 5 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 5 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.MainBuilding, Level = 5 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 6 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 6 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 7 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 7 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Residence, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Barracks, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.MainBuilding, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Warehouse, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Granary, Level = 20 });
        }
        public static void SupplyVillagePlan(Account acc, Village vill)
        {
            FarmVillagePlan(acc, vill);
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Marketplace, Level = 20 });
            //market center?
        }
        public static void OffVillagePlan(Account acc, Village vill)
        {
            DeffVillagePlan(acc, vill);
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Academy, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Stable, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.Workshop, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.RallyPoint, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingType.General, Building = BuildingEnum.TournamentSquare, Level = 1 });
        }
        public static void SetDefaultTransitConfiguration(Account acc, Village vill)
        {
            var res = vill.Market.Settings.Configuration;
            var transit = new Resources();
            var limit = new Resources();
            transit.Wood = 90; //%
            transit.Clay = 90;
            transit.Iron = 90;
            transit.Crop = 90;
            limit.Wood = 20000;
            limit.Clay = 20000;
            limit.Iron = 20000;
            limit.Crop = 15000;
            res.Enabled = true;
            res.BalanceType = Tasks.ResourcesConfiguration.BalanceType.RecieveFrom;
            res.FillLimit = limit;
            res.TargetLimit = transit;
        }
    }
}
