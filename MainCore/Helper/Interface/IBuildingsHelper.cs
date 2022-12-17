using MainCore.Enums;
using System.Collections.Generic;

namespace MainCore.Helper.Interface
{
    public interface IBuildingsHelper
    {
        bool CanBuild(int villageId, BuildingEnums building);

        int GetDorf(int index);

        List<BuildingEnums> GetCanBuild(int accountId, int villageId);
    }
}