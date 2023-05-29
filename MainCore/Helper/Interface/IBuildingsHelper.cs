using MainCore.Enums;
using System.Collections.Generic;

namespace MainCore.Helper.Interface
{
    public interface IBuildingsHelper
    {
        int GetDorf(int index);

        int GetDorf(BuildingEnums building);

        List<BuildingEnums> GetCanBuild(int villageId);

        bool IsAutoCropFieldAboveLevel(int villageId, int level);

        bool IsAutoResourceFieldAboveLevel(int villageId, int level);

        bool IsBuildingAboveLevel(int villageId, BuildingEnums building, int level);

        bool IsExists(int villageId, BuildingEnums building);

        bool CanBuild(int villageId, TribeEnums tribe, BuildingEnums building);
    }
}