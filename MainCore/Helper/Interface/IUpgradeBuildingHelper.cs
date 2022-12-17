using FluentResults;
using MainCore.Models.Runtime;

namespace MainCore.Helper.Interface
{
    public interface IUpgradeBuildingHelper
    {
        PlanTask ExtractResField(int villageId, PlanTask buildingTask);

        Result<PlanTask> NextBuildingTask(int accountId, int villageId);

        void RemoveFinishedCB(int villageId);
    }
}