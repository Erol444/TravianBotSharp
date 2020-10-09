using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Tasks;

namespace TravBotSharp.Files.Helpers
{
    public static class DefaultConfigurations
    {
        public static void DeffVillagePlan(Account acc, Village vill)
        {
            FarmVillagePlan(acc, vill);
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Barracks, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Academy, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Smithy, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Marketplace, Level = 15 });
        }
        public static void FarmVillagePlan(Account acc, Village vill)
        {
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.AutoUpgradeResFields, Level = 10, ResourceType = ResTypeEnum.AllResources, BuildingStrategy = BuildingStrategyEnum.BasedOnRes });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.RallyPoint, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.MainBuilding, Level = 3 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 3 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 3 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Marketplace, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 4 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 4 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 5 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 5 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.MainBuilding, Level = 5 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 6 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 6 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 7 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 7 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Residence, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Barracks, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.MainBuilding, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 10 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Warehouse, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Granary, Level = 20 });
        }
        public static void SupplyVillagePlan(Account acc, Village vill)
        {
            FarmVillagePlan(acc, vill);
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Marketplace, Level = 20 });
            //market center?
        }
        public static void OffVillagePlan(Account acc, Village vill)
        {
            DeffVillagePlan(acc, vill);
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Academy, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Stable, Level = 20 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.Workshop, Level = 1 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.RallyPoint, Level = 15 });
            BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask() { TaskType = BuildingHelper.BuildingType.General, Building = Classificator.BuildingEnum.TournamentSquare, Level = 1 });
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
            limit.Wood = 40000;
            limit.Clay = 40000;
            limit.Iron = 40000;
            limit.Crop = 30000;
            res.Enabled = true;
            res.BalanceType = Tasks.ResourcesConfiguration.BalanceType.RecieveFrom;
            res.FillLimit = limit;
            res.TargetLimit = transit;
        }
    }
}
