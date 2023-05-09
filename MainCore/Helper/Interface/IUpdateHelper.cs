using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IUpdateHelper
    {
        Result UpdateAdventures();

        Result UpdateCurrentlyBuilding();

        Result UpdateDorf1();

        Result UpdateDorf2();

        Result UpdateFarmList();

        Result UpdateHeroInventory();

        Result UpdateProduction();

        Result UpdateResource();
    }
}