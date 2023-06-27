using MainCore.Models.Database;
using MainCore.Models.Runtime;
using System.Collections.Generic;

namespace MainCore.Helper.Interface
{
    public interface IDatabaseHelper
    {
        List<Access> GetAccesses(int accountId);
        Account GetAccount(int accountId);
        AccountSetting GetAccountSetting(int accountId);
        List<Adventure> GetAdventures(int accountId);
        VillageBuilding GetCropLand(int villageId);
        Hero GetHero(int accountId);
        (PlanTask, VillCurrentBuilding) GetInProgressBuilding(int villageId, int buildingId);
        Village GetVillage(int villageId);
        List<VillageBuilding> GetVillageBuildings(int villageId);
        List<VillCurrentBuilding> GetVillageCurrentlyBuildings(int villageId);
    }
}