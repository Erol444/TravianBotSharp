using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IUpdateHelper
    {
        void Update(int accountId, int villageId = -1);

        void UpdateAccountInfo(int accountId);

        void UpdateAdventures(int accountId);

        void UpdateBuildings(int accountId, int villageId);

        Result UpdateCurrentlyBuilding(int accountId, int villageId);

        Result UpdateDorf1(int accountId, int villageId);

        Result UpdateDorf2(int accountId, int villageId);

        void UpdateFarmList(int accountId);

        void UpdateHeroInfo(int accountId);

        void UpdateHeroInventory(int accountId);

        void UpdateProduction(int accountId, int villageId);

        void UpdateResource(int accountId, int villageId);

        void UpdateResourceFields(int accountId, int villageId);

        void UpdateVillageList(int accountId);
    }
}