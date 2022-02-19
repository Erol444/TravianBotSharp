using TbsCore.Tasks;
using TbsReact.Models;
using TbsReact.Models.Villages;

namespace TbsReact.Extension
{
    public static class VillageExtension
    {
        public static Village GetInfo(this TbsCore.Models.VillageModels.Village village)
        {
            return new Village
            {
                Id = village.Id,
                Name = village.Name,
                Coordinate = new Coordinate
                {
                    X = village.Coordinates.x,
                    Y = village.Coordinates.y,
                },
            };
        }

        public static Building GetInfo(this TbsCore.Models.VillageModels.Building building)
        {
            return new Building
            {
                Name = building.Type.ToString(),
                Location = building.Id,
                Level = building.Level,
                UnderConstruction = building.UnderConstruction,
            };
        }

        public static TaskBuilding GetInfo(this TbsCore.Models.BuildingModels.BuildingTask task)
        {
            string Name;
            if (task.TaskType == TbsCore.Helpers.Classificator.BuildingType.AutoUpgradeResFields)
            {
                Name = AutoBuildResFieldsStr(task);
            }
            else
            {
                Name = TbsCore.Helpers.VillageHelper.BuildingTypeToString(task.Building);
            }
            return new TaskBuilding
            {
                Name = Name,
                Level = task.Level,
                Location = task.BuildingId ?? -1,
            };
        }

        public static CurrentBuilding GetInfo(this TbsCore.Models.VillageModels.BuildingCurrently building)
        {
            return new CurrentBuilding
            {
                Name = building.Building.ToString(),
                Level = building.Level,
                CompleteTime = building.Duration,
            };
        }

        private static string AutoBuildResFieldsStr(TbsCore.Models.BuildingModels.BuildingTask task)
        {
            var str = "";
            switch (task.ResourceType)
            {
                case ResTypeEnum.AllResources:
                    str += "All fields";
                    break;

                case ResTypeEnum.ExcludeCrop:
                    str += "Exclude crop";
                    break;

                case ResTypeEnum.OnlyCrop:
                    str += "Only crop";
                    break;
            }
            str += "-";
            switch (task.BuildingStrategy)
            {
                case BuildingStrategyEnum.BasedOnLevel:
                    str += "Based on level";
                    break;

                case BuildingStrategyEnum.BasedOnProduction:
                    str += "Based on production";
                    break;

                case BuildingStrategyEnum.BasedOnRes:
                    str += "Based on storage";
                    break;
            }
            return str;
        }
    }
}