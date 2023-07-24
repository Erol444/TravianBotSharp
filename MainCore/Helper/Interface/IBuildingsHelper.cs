using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;
using System.Collections.Generic;

namespace MainCore.Helper.Interface
{
    public interface IBuildingsHelper
    {
        int GetDorf(int index);

        int GetDorf(BuildingEnums building);

        List<BuildingEnums> GetCanBuild(int villageId);

        Result<bool> IsPrerequisiteAvailable(int villageId, PlanTask task);

        bool IsTaskComplete(int villageId, PlanTask task);

        Result<bool> IsMultipleReady(int villageId, PlanTask task);
    }
}