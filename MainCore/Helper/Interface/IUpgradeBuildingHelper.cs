using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;

namespace MainCore.Helper.Interface
{
    public interface IUpgradeBuildingHelper
    {
        Result<bool> IsNeedAdsUpgrade(int accountId, int villageId, PlanTask buildingTask);

        bool IsEnoughFreeCrop(int villageId, BuildingEnums building);

        public Result<bool> GotoBuilding(int accountId, int villageId, PlanTask buildingTask);

        bool IsEnoughResource(int accountId, int villageId, BuildingEnums building, bool isNewBuilding);

        long[] GetResourceMissing(int accountId, int villageId, BuildingEnums building, bool isNewBuilding);

        PlanTask ExtractResField(int villageId, PlanTask buildingTask);

        void RemoveFinishedCB(int villageId);

        Result Construct(int accountId, PlanTask buildingTask);

        Result Upgrade(int accountId, PlanTask buildingTask);

        Result UpgradeAds(int accountId, PlanTask buildingTask);

        PlanTask GetFirstTask(int villageId);

        PlanTask GetFirstBuildingTask(int villageId);

        PlanTask GetFirstResTask(int villageId);
    }
}