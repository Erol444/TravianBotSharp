namespace TbsCrossPlatform.Models.Enums
{
    public enum BuildingType
    {
        General,
        AutoUpgradeResFields
    }

    public enum ResTypeEnum
    {
        AllResources = 0,
        ExcludeCrop,
        OnlyCrop
    }

    public enum BuildingStrategyEnum
    {
        BasedOnRes = 0,
        BasedOnLevel,
        BasedOnProduction
    }
}