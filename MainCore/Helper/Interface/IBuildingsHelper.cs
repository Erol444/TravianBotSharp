using MainCore.Enums;

namespace MainCore.Helper.Interface
{
    public interface IBuildingsHelper
    {
        bool CanBuild(int villageId, BuildingEnums building);

        bool CanBuild(int villageId, TribeEnums tribe, BuildingEnums building);

        int GetDorf(BuildingEnums building);

        int GetDorf(int index);
    }
}