using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;

namespace MainCore.Helper.Interface
{
    public interface IUpgradeBuildingHelper
    {
        Result<bool> IsNeedAdsUpgrade(PlanTask buildingTask);

        bool IsEnoughFreeCrop(BuildingEnums building);

        public Result<bool> GotoBuilding(PlanTask buildingTask);

        bool IsEnoughResource(BuildingEnums building, bool isNewBuilding);

        long[] GetResourceMissing(BuildingEnums building, bool isNewBuilding);

        PlanTask ExtractResField(PlanTask buildingTask);

        void RemoveFinishedCB();

        Result Construct(PlanTask buildingTask);

        Result Upgrade(PlanTask buildingTask);

        Result UpgradeAds(PlanTask buildingTask);

        PlanTask GetFirstTask();

        PlanTask GetFirstBuildingTask();

        PlanTask GetFirstResTask();
    }
}