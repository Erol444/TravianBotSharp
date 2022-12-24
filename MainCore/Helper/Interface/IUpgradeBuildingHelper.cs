using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;

namespace MainCore.Helper.Interface
{
    public interface IUpgradeBuildingHelper
    {
        Result<bool> IsNeedAdsUpgrade(int accountId, int villageId, PlanTask buildingTask);

        public Result<bool> IsBuildingCompleted(int accountId, int villageId, int buildingId, int buildingLevel)


        bool IsEnoughFreeCrop(int villageId, BuildingEnums building);

        public Result<bool> GotoBuilding(int accountId, int villageId, PlanTask buildingTask)


        bool IsEnoughResource(int accountId, int villageId, BuildingEnums building, bool isNewBuilding);

        long[] GetResourceMissing(int accountId, int villageId, BuildingEnums building, bool isNewBuilding);

        PlanTask ExtractResField(int villageId, PlanTask buildingTask);

        Result<PlanTask> NextBuildingTask(int accountId, int villageId);

        void RemoveFinishedCB(int villageId);

        Result Construct(int accountId, PlanTask buildingTask);

        Result Upgrade(int accountId, PlanTask buildingTask);

        Result UpgradeAds(int accountId, PlanTask buildingTask);
    }
}