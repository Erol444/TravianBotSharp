using MainCore.Enums;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IBuildingsHelper
    {
        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        bool CanBuild(BuildingEnums building);

        int GetDorf(int index);

        int GetDorf(BuildingEnums building);

        List<BuildingEnums> GetCanBuild();
    }
}