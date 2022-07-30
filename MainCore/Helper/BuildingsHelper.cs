using MainCore.Enums;

namespace MainCore.Helper
{
    public static class BuildingsHelper
    {
        public static int GetDorf(int index) => index < 19 ? 1 : 2;

        public static int GetDorf(BuildingEnums building) => GetDorf((int)building);
    }
}