using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IUpdateHelper
    {
        Result UpdateAdventures(int accountId);
        Result UpdateCurrentlyBuilding(int accountId, int villageId);
        Result UpdateDorf1(int accountId, int villageId);
        Result UpdateDorf2(int accountId, int villageId);
        Result UpdateFarmList(int accountId);
        Result UpdateHeroInventory(int accountId);
        Result UpdateProduction(int accountId, int villageId);
        Result UpdateResource(int accountId, int villageId);
    }
}